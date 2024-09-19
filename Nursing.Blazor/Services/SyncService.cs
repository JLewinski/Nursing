using Nursing.Core.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Result = (bool success, string message);

namespace Nursing.Blazor.Services;

internal class SyncService
{
    private readonly HttpClient _httpClient;
    private readonly LocalDatabase _database;
    private readonly CacheService _cacheService;

    public SyncService(HttpClient httpClient, LocalDatabase database, CacheService cacheService)
    {
        _httpClient = httpClient;
        _database = database;
        _cacheService = cacheService;
    }

    public async Task<bool> IsLoggedIn()
    {
        var settings = await _cacheService.GetSettings();
        return settings.Token != null;
    }

    public async Task<bool> IsAdmin()
    {
        var settings = await _cacheService.GetSettings();
        return settings.Token != null && settings.IsAdmin;
    }

    public async Task<string> GetUsername()
    {
        if (!await IsLoggedIn())
        {
            throw new Exception("User is not logged in");
        }

        var settings = await _cacheService.GetSettings();
        return settings.Username!;
    }

    public async Task<Result> ChangePassword(string currentPassword, string newPassword)
    {
        HttpResponseMessage result;
        try
        {
            result = await Request("Account/changePassword", new ChangePasswordModel { CurrentPassword = currentPassword, NewPassword = newPassword });
        }
        catch
        {
            return (false, "Could not connect to server");
        }

        if (result.IsSuccessStatusCode)
        {
            return (true, "Password changed successfully");
        }

        return (false, "Could not change password");
    }

    private async Task ApplyBearer()
    {
        var settings = await _cacheService.GetSettings();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Token);
    }

    private async Task<HttpResponseMessage> Request(string path, object data)
    {
        await ApplyBearer();
        return await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + path), data);
    }

    private async Task<Result> Refresh()
    {
        var settings = await _cacheService.GetSettings();
        if (settings.RefreshToken == null)
        {
            return (false, "User is not logged in");
        }

        var result = await Request("Account/refreshToken", settings.RefreshToken);
        var (success, _) = await ReadAuthResult(result, settings.Username);
        if (success)
        {
            return (true, string.Empty);
        }
        else
        {
            settings.Token = null;
            settings.RefreshToken = null;
            await _cacheService.SaveSettings(settings);
        }
        return (false, "User has been logged out");
    }

    public async Task<Result> Sync(bool afterRefresh = false)
    {
        var syncDate = DateTime.UtcNow;

        var settings = await _cacheService.GetSettings();

        if (settings.Token == null)
        {
            return (false, "Not logged in");
        }
        var feedingsUp = await _database.GetUpdatedFeedings(settings.LastSync);


        var data = new SyncModel { LastSync = settings.LastSync, Feedings = feedingsUp };

        HttpResponseMessage result;
        try
        {
            result = await Request("Sync/sync", data);
        }
        catch
        {
            return (false, "Could not connect to server");
        }

        if (!result.IsSuccessStatusCode)
        {
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized && !afterRefresh && (await Refresh()).success)
            {
                return await Sync(true);
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return (false, "User is not logged in");
            }

            return (false, "Could not sync");
        }

        bool updated = false;
        var resultData = await result.Content.ReadFromJsonAsync<SyncResult>();
        if (resultData?.Success == true)
        {
            updated = resultData.Feedings.Count > 0 || resultData.Updates > 0;
            await _database.SaveUpdated(resultData.Feedings, settings.LastSync);
            settings.LastSync = DateTime.UtcNow;
            await _cacheService.SaveSettings(settings);
        }

        return (updated, updated ? "Synced successfully" : "No new data");
    }

    private async Task<(bool success, string message)> ReadAuthResult(HttpResponseMessage result, string? username)
    {
        if (!result.IsSuccessStatusCode)
        {
            var message = await result.Content.ReadAsStringAsync();
            return (false, message);
        }

        var token = await result.Content.ReadFromJsonAsync<SignInResult>();
        if (token == null)
        {
            return (false, "Something went wrong when signing in. Please contact support to solve this issue.");
        }

        var settings = await _cacheService.GetSettings();
        settings.Token = token.AuthToken;
        settings.RefreshToken = token.RefreshToken;
        settings.IsAdmin = token.IsAdmin;
        if (username != null)
        {
            settings.Username = username;
        }

        await _cacheService.SaveSettings(settings);
        return (true, "Logged In");
    }

    public async Task<(bool success, string message)> Login(string username, string password, bool rememberMe)
    {
        LoginModel loginModel = new() { Username = username, Password = password, RememberMe = rememberMe };

        try
        {
            var result = await Request("Account/login", loginModel);
            return await ReadAuthResult(result, username);
        }
        catch
        {
            return (false, "Could not connect to server. Please try again later");
        }
    }

    public async Task<bool> Logout()
    {
        await ApplyBearer();
        try
        {
            await _httpClient.GetAsync(new Uri(ApiOptions.RootUrl + "Account/logout"));
        }
        catch { }
        var settings = await _cacheService.GetSettings();
        settings.Logout();
        await _cacheService.SaveSettings(settings);
        return true;
    }

    public async Task<Result> Register(string username, string password, bool isAdmin)
    {
        RegisterModel registerModel = new() { Username = username, Password = password, IsAdmin = isAdmin };
        HttpResponseMessage result;
        try
        {
            result = await Request("Account/register", registerModel);
        }
        catch
        {
            return (false, "Could not connect to server. Please try again later");
        }

        if (result.IsSuccessStatusCode)
        {
            return (true, $"{username} has been registered");
        }
        return (false, $"{username} could not be registered");
    }

    public async Task<Result> Delete(string username)
    {
        await ApplyBearer();
        HttpResponseMessage result;
        try
        {

            result = await _httpClient.DeleteAsync(new Uri(ApiOptions.RootUrl + $"Account/delete/{username}"));
        }
        catch
        {
            return (false, "Could not connect to server");
        }

        if (result.IsSuccessStatusCode)
        {
            var settings = await _cacheService.GetSettings();
            if (settings.Username == username)
            {
                settings.RememberMe = false;
                settings.Logout();
                await _cacheService.SaveSettings(settings);
            }

            return (true, await result.Content.ReadAsStringAsync());
        }
        else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return (false, $"You do not have permission to delete {username}");
        }
        return (false, $"{username} could not be deleted or could not be found");
    }

    public async Task<List<InviteViewModel>> GetInvites()
    {
        await ApplyBearer();
        var result = await _httpClient.GetAsync(new Uri(ApiOptions.RootUrl + "Sync/invites"));

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<List<InviteViewModel>>() ?? [];
        }

        return [];
    }

    public async Task<Result> SendInvite(string username)
    {
        await ApplyBearer();
        HttpResponseMessage result;
        try
        {
            result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "Sync/sendInvite"), username);
        }
        catch
        {
            return (false, "Could not connect to server");
        }
        if (result.IsSuccessStatusCode)
        {
            return (true, "Invite Sent");
        }
        return (false, "Could not send invite");
    }

    public async Task<Result> AcceptInvite(Guid groupId)
    {
        await ApplyBearer();
        HttpResponseMessage result;
        try
        {
            result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "Sync/acceptInvite"), groupId);
        }
        catch
        {
            return (false, "Could not connect to server");
        }
        if (result.IsSuccessStatusCode)
        {
            return (true, await result.Content.ReadAsStringAsync());
        }
        return (false, "Could not accept invite");
    }

    public async Task<Result> DeclineInvite(Guid groupId)
    {
        await ApplyBearer();
        HttpResponseMessage result;
        try
        {
            result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "Sync/declineInvite"), groupId);
        }
        catch
        {
            return (false, "Could not connect to server");
        }
        if (result.IsSuccessStatusCode)
        {
            return (true, await result.Content.ReadAsStringAsync());
        }
        return (false, "Could not decline invite");
    }

    public async Task<List<SimpleUser>> GetUsers()
    {
        await ApplyBearer();
        var result = await _httpClient.GetAsync(new Uri(ApiOptions.RootUrl + "Account/users"));
        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<List<SimpleUser>>() ?? [];
        }
        return [];
    }
}

public static class ApiOptions
{
#if DEBUG
    public const string RootUrl = "https://localhost:7238/api/";
#elif TEST
    public const string RootUrl = "https://nursing-test.lewinskitech.com/api/";
#elif RELEASE
    public const string RootUrl = "https://nursing.lewinskitech.com/api/";
#endif
}
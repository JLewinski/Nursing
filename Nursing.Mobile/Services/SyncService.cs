using Nursing.Core.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Result = (bool success, string message);

namespace Nursing.Mobile.Services;

public class SyncService
{
    private readonly HttpClient _httpClient;
    private readonly Nursing.Services.EFDatabase _database;
    private readonly CacheService _cacheService;

    public SyncService(HttpClient httpClient, Nursing.Services.EFDatabase database, CacheService cacheService)
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

    public async Task<Result> ChangePassword(string currentPassword, string newPassword)
    {
        var settings = await _cacheService.GetSettings();
        var result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "/Account/changePassword"), new ChangePasswordModel { CurrentPassword = currentPassword, NewPassword = newPassword });
        if (result.IsSuccessStatusCode)
        {
            return (true, "Password changed successfully");
        }
        return (false, "Could not change password");
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

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Token);

        var data = new SyncModel { LastSync = settings.LastSync, Feedings = feedingsUp };

        var result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "/Sync/sync"), data);

        if (!result.IsSuccessStatusCode)
        {
            if (settings.RefreshToken == null || afterRefresh)
            {
                return (false, "User is not logged in");
            }

            result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "/Account/refreshToken"), settings.RefreshToken);
            var (success, _) = await this.ReadAuthResult(result);
            if (success)
            {
                return await Sync(true);
            }
            else
            {
                settings.Token = null;
                settings.RefreshToken = null;
                await _cacheService.SaveSettings(settings);
            }
            return (false, "User has been logged out");

        }

        bool updated = false;
        var resultData = await result.Content.ReadFromJsonAsync<SyncResult>();
        if (resultData?.Success == true && resultData.Feedings.Count > 0)
        {
            updated = true;
            await _database.SaveUpdated(resultData.Feedings);
        }

        settings.LastSync = syncDate;
        await _cacheService.SaveSettings(settings);

        return (updated, updated ? "Synced successfully" : "No new data");
    }

    private async Task<(bool success, string message)> ReadAuthResult(HttpResponseMessage result)
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

        await _cacheService.SaveSettings(settings);
        return (true, "Logged In");
    }

    public async Task<(bool success, string message)> Login(string username, string password, bool rememberMe)
    {
        LoginModel loginModel = new() { Username = username, Password = password, RememberMe = rememberMe };

        try
        {
            var result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "/Account/login"), loginModel);
            return await ReadAuthResult(result);
        }
        catch
        {
            return (false, "Could not connect to server. Please try again later");
        }
    }

    public async Task<bool> Logout()
    {
        var settings = await _cacheService.GetSettings();
        settings.Token = null;
        await _cacheService.SaveSettings(settings);
        return true;
    }

    public async Task<bool> Register(string username, string password)
    {
        AccountModel registerModel = new() { Username = username, Password = password };
        var result = await _httpClient.PostAsJsonAsync(new Uri(ApiOptions.RootUrl + "/Account/register"), registerModel);
        if (result.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }
}

public static class ApiOptions
{
#if DEBUG
    public const string RootUrl = "https://localhost:7238";
#elif TEST
    public const string RootUrl = "https://localhost:7238";
#elif RELEASE
    public const string RootUrl = "https://nursing-h2azd7b5f6gnd0dz.eastus-01.azurewebsites.net";
#endif
}
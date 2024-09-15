﻿using Nursing.Core.Models;
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
        var result = await Request("Account/changePassword", new ChangePasswordModel { CurrentPassword = currentPassword, NewPassword = newPassword });
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

        var result = await Request("Sync/sync", data);

        if (!result.IsSuccessStatusCode)
        {
            if (!afterRefresh && (await Refresh()).success)
            {
                return await Sync(true);
            }

            return (false, "User is not logged in");
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
        await _httpClient.GetAsync(new Uri(ApiOptions.RootUrl + "Account/logout"));
        var settings = await _cacheService.GetSettings();
        settings.Logout();
        await _cacheService.SaveSettings(settings);
        return true;
    }

    public async Task<Result> Register(string username, string password, bool isAdmin)
    {
        RegisterModel registerModel = new() { Username = username, Password = password, IsAdmin = isAdmin };
        var result = await Request("Account/register", registerModel);
        if (result.IsSuccessStatusCode)
        {
            return (true, $"{username} has been registered");
        }
        return (false, $"{username} could not be registered");
    }

    public async Task<Result> Delete(string username)
    {
        await ApplyBearer();
        var result = await _httpClient.DeleteAsync(new Uri(ApiOptions.RootUrl + $"Account/delete/{username}"));
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
}

public static class ApiOptions
{
#if DEBUG
    public const string RootUrl = "https://localhost:7238/";
#elif TEST
    public const string RootUrl = "https://localhost:7238/";
#elif RELEASE
    public const string RootUrl = "https://nursing-h2azd7b5f6gnd0dz.eastus-01.azurewebsites.net/";
#endif
}
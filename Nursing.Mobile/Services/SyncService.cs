using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nursing.Core.Models;
using Nursing.Core.Models.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

    public async Task<(bool success, string message)> Sync(bool afterRefresh = false)
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
            var outcome = await this.ReadResult(result);
            if (outcome.success)
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

    private async Task<(bool success, string message)> ReadResult(HttpResponseMessage result)
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
            return await ReadResult(result);
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
    public const string RootUrl = "https://nursingapi.lewinskitech.com";
#endif
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nursing.Core.Models;
using Nursing.Core.Models.DTO;
using Nursing.Core.Services;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Nursing.Mobile.Services;

public class SyncService
{
    private readonly HttpClient _httpClient;
    private readonly Nursing.Core.Services.IDatabase _database;
    private readonly CacheService _cacheService;
    private readonly SyncOptions _settings;

    public SyncService(HttpClient httpClient, Nursing.Services.EFDatabase database, IOptions<SyncOptions> settings, CacheService cacheService)
    {
        _httpClient = httpClient;
        _database = database;
        _cacheService = cacheService;
        _settings = settings.Value;
    }

    public async Task<bool> IsLoggedIn()
    {
        var settings = await _cacheService.GetSettings();
        return settings.Token != null;
    }

    public async Task<bool> Sync()
    {
        var syncDate = DateTime.UtcNow;

        var settings = await _cacheService.GetSettings();

        if(settings.Token == null)
        {
            return false;
        }
        var feedingsUp = await _database.GetUpdatedFeedings(settings.LastSync);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.Token);

        var data = new SyncModel { LastSync = settings.LastSync, Feedings = feedingsUp };

        var result = await _httpClient.PostAsJsonAsync(new Uri(_settings.RootUrl + "/Sync/sync"), data);

        if (!result.IsSuccessStatusCode)
        {
            return false;
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

        return updated;
    }

    public async Task<bool> Login(string username, string password, bool rememberMe)
    {
        LoginModel loginModel = new() { Username = username, Password = password, RememberMe = rememberMe };
        
        var result = await _httpClient.PostAsJsonAsync(new Uri(_settings.RootUrl + "/Account/login"), loginModel);
        if (result.IsSuccessStatusCode)
        {
            var token = await result.Content.ReadAsStringAsync();
            var settings = await _cacheService.GetSettings();
            settings.Token = token;
            await _cacheService.SaveSettings(settings);
            return true;
        }
        return false;
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
        RegisterModel registerModel = new() { Username = username, Password = password };
        var result = await _httpClient.PostAsJsonAsync(new Uri(_settings.RootUrl + "/Account/register"), registerModel);
        if (result.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }
}

public class SyncOptions
{
    public required string RootUrl { get; set; }
}
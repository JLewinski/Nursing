using Blazored.LocalStorage;
using Nursing.Mobile.Models;
using Nursing.Models;

namespace Nursing.Blazor.Services;

public class CacheService
{
    private static string CachePath => "NursingCache";
    private static string SettingsPath => "NursingSettings";

    private readonly ILocalStorageService _localStorageService;

    public CacheService(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    private FeedingCache? feedingCache;
    private Settings? settingsCache;

    public async Task<FeedingCache> RefreshCache(LocalDatabase localDatabase)
    {
        var feedings = await localDatabase.GetFeedings(DateTime.UtcNow.Date.AddDays(-1));
        if (feedings.Count == 0)
        {
            return feedingCache ??= await Get();
        }

        if (feedings.Count > 1)
        {
            await Cache(new(feedings[1]));
        }

        return await Cache(new(feedings[0]));
    }

    public async Task<FeedingCache> Cache(Feeding feeding)
    {
        feedingCache ??= await Get();

        if (feeding.IsFinished)
        {
            feedingCache.CurrentFeeding = new();
            feedingCache.LastStart = feeding.Started;
            feedingCache.LastWasLeft = feeding.LastIsLeft;
        }
        else
        {
            feedingCache.CurrentFeeding = feeding;
        }

        await _localStorageService.SetItemAsync(CachePath, feedingCache);
        return feedingCache;
    }

    public async Task DeleteCache()
    {
        feedingCache = new();
        await _localStorageService.SetItemAsync(CachePath, feedingCache);
        settingsCache!.LastSync = DateTime.MinValue;
        await SaveSettings(settingsCache);
    }

    private async Task<T> GetFromLocalStorage<T>(string path) where T : new()
    {
        var data = await _localStorageService.GetItemAsync<T>(path);
        return data ?? new();
    }

    public async Task<FeedingCache> Get()
    {
        return feedingCache ??= await GetFromLocalStorage<FeedingCache>(CachePath);
    }

    public async Task<Settings> GetSettings()
    {
        return settingsCache ??= await GetFromLocalStorage<Settings>(SettingsPath);
    }

    public async Task SaveSettings(Settings settings)
    {
        await _localStorageService.SetItemAsync(SettingsPath, settings);
    }
}

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

    public static Task<FeedingCache> RefreshCache()
    {
        throw new NotImplementedException();
    }

    public async Task Cache(Feeding feeding)
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
        return data ?? new ();
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

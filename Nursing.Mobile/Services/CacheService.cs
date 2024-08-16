using Nursing.Mobile.Models;
using Nursing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nursing.Mobile.Services;

public class CacheService
{
    private static string CachePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Nursing.json");
    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Settings.json");
    private FeedingCache? feedingCache;
    private Settings? settingsCache;

    public CacheService()
    {

    }

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

        var json = JsonSerializer.Serialize(feedingCache);
        await File.WriteAllTextAsync(CachePath, json);
    }

    private static async Task<T> GetFromFile<T>(string path) where T : new()
    {
        if (!File.Exists(path))
        {
            return new();
        }

        try
        {

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<T>(json) ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<FeedingCache> Get()
    {
        return feedingCache ??= await GetFromFile<FeedingCache>(CachePath);
    }

    public async Task<Settings> GetSettings()
    {
        return settingsCache ??= await GetFromFile<Settings>(SettingsPath);
    }

    public async Task SaveSettings(Settings settings)
    {
        settingsCache = settings;
        var json = JsonSerializer.Serialize(settingsCache);
        await File.WriteAllTextAsync(SettingsPath, json);
    }
}

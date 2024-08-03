using Nursing.Models;
using System.Text.Json;

namespace Nursing.Mobile.Services;

internal class CacheDatabase : IDatabase
{
    private const string DatabaseFilename = "Nursing.json";
    private static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);

    private const string SettingsFilename = "Settings.json";
    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SettingsFilename);

    public CacheDatabase()
    {
        //File.Delete(DatabasePath);
    }

    private async Task Init()
    {
        if (File.Exists(DatabasePath))
        {
            return;
        }

        await File.WriteAllTextAsync(DatabasePath, JsonSerializer.Serialize(new List<Feeding>()));
    }

    public async Task Delete(Guid id)
    {
        var all = await GetFeedings();
        var feeding = all.First(x => x.Id == id);
        all.Remove(feeding);
        await Save(all);
    }

    private async Task Save(List<Feeding> feedings)
    {
        var json = JsonSerializer.Serialize(feedings);
        await File.WriteAllTextAsync(DatabasePath, json);
    }

    public async Task<(TimeSpan averageTotal, TimeSpan averageRight, TimeSpan averageLeft)> GetAverages(DateTime? start, DateTime? end)
    {
        var data = await GetFeedings(start, end);
        var (total, right, left) = data.Aggregate((total: TimeSpan.Zero, right: TimeSpan.Zero, left: TimeSpan.Zero), (acc, x) =>
        {
            acc.total += x.TotalTime;
            acc.right += x.RightBreastTotal;
            acc.left += x.LeftBreastTotal;
            return acc;
        });

        return (total / data.Count, right / data.Count, left / data.Count);
    }

    public async Task<List<Feeding>> GetFeedings(DateTime? start = null, DateTime? end = null)
    {
        await Init();

        var jsonData = await File.ReadAllTextAsync(DatabasePath);

        if (jsonData == string.Empty)
        {
            return [];
        }

        var feedings = JsonSerializer.Deserialize<List<Feeding>>(jsonData);
        return feedings?
            .Where(x => x.LastFinish >= (start ?? DateTime.MinValue) && x.LastFinish <= (end ?? DateTime.UtcNow))
            .OrderByDescending(x => x.LastFinish)
            .ToList() ?? [];
    }

    public async Task<List<Feeding>> GetLast()
    {
        var all = await GetFeedings();
        return all.Take(2).ToList();
    }

    public async Task<bool> SaveFeeding(Feeding feeding)
    {
        var all = await GetFeedings();
        var existing = all.FirstOrDefault(x => x.Id == feeding.Id);
        
        if (existing is not null)
        {
            all.Remove(existing);
        }
        else
        {
            feeding.Id = Guid.NewGuid();
        }

        all.Add(feeding);
        await Save(all);

        return true;
    }

    public async Task<TimeSpan> GetInBetween()
    {
        Settings settings;

        if (!File.Exists(SettingsPath))
        {
            settings = new();
        }
        else
        {
            var data = await File.ReadAllTextAsync(SettingsPath);
            settings = JsonSerializer.Deserialize<Settings>(data) ?? new();
        }

        return settings.Duration;
    }
}

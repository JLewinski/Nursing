using Nursing.Models;
using System.Text.Json;

namespace Nursing.Mobile.Services;

internal class CacheDatabase : IDatabase
{
    private static string DatabaseFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Nursing");
    private static string DatabasePath => CreateDatabasePath(DateTime.UtcNow);

    private const string SettingsFilename = "Settings.json";
    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SettingsFilename);

    private static string CreateDatabasePath(DateTime date)
    {
        return Path.Combine(DatabaseFolderPath, $"Data.{date:yyyy.MM.dd}.json");
    }

    public CacheDatabase()
    {
        
    }

    private static bool IsInit(string? dbPath = null)
    {
        if (!File.Exists(dbPath ?? DatabasePath))
        {
            Directory.CreateDirectory(DatabaseFolderPath);
            return false;
        }

        return true;
    }

    public async Task Delete(Feeding feeding)
    {
        var path = CreateDatabasePath(feeding.Started);

        var all = await GetFeedings(path);

        var temp = all.FirstOrDefault(x => x.Id == feeding.Id);
        if (temp is null)
        {
            return;
        }

        all.Remove(temp);
        await Save(all, path);
    }

    private static async Task Save(IEnumerable<Feeding> feedings, string path)
    {
        var json = JsonSerializer.Serialize(feedings);

        await File.WriteAllTextAsync(path, json);
    }

    private static async Task Save(List<Feeding> feedings)
    {
        feedings.Sort((x, y) => x.Started.CompareTo(y.Started));

        var totalCount = feedings.Count;

        while (totalCount > 0)
        {
            var last = feedings.Last().Started.Date;
            await Save(feedings.Where(x => x.Started.Date == last), CreateDatabasePath(last));
        }
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

    public async Task<List<Feeding>> GetFeedings(string fileName)
    {
        if (!IsInit(fileName))
        {
            return [];
        }

        var jsonData = await File.ReadAllTextAsync(fileName);

        if (jsonData == string.Empty)
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<Feeding>>(jsonData) ?? [];
    }

    public async Task<List<Feeding>> GetFeedings(DateTime? start = null, DateTime? end = null)
    {
        if (start is null && end is null && !IsInit())
        {
            return [];
        }

        List<Feeding> feedings = [];

        if (start is null)
        {
            feedings = await GetFeedings(DatabasePath);
            feedings.AddRange(await GetFeedings(CreateDatabasePath(DateTime.UtcNow.AddDays(-1))));
        }
        else
        {
            var dateIterator = start.Value.Date;

            while (dateIterator <= (end ?? DateTime.UtcNow).Date)
            {
                feedings.AddRange(await GetFeedings(CreateDatabasePath(dateIterator)));

                dateIterator = dateIterator.AddDays(1);
            }
        }


        return feedings?
            .Where(x => x.Started >= (start ?? DateTime.MinValue) && x.Started <= (end ?? DateTime.UtcNow))
            .OrderByDescending(x => x.Started)
            .ToList() ?? [];
    }

    public async Task<List<Feeding>> GetLast()
    {
        var all = await GetFeedings();
        return all.Take(2).ToList();
    }

    public async Task<bool> SaveFeeding(Feeding feeding)
    {
        var path = CreateDatabasePath(feeding.Started);
        var all = await GetFeedings(path);
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

        await Save(all, path);

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

    public Task DeleteAll()
    {
        Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Nursing"), true);
        return Task.FromResult(true);
    }
}

using Nursing.Models;
using System.Text.Json;

namespace Nursing.Sqlite.Services;

internal class CacheDatabase
{
    private static string DatabaseFolderPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Nursing");
    private static string DatabasePath => CreateDatabasePath(DateTime.UtcNow);

    private const string SettingsFilename = "Settings.json";
    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SettingsFilename);

    private static string CreateDatabasePath(DateTime date)
    {
        return Path.Combine(DatabaseFolderPath, $"Data.{date:yyyy.MM.dd}.json");
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

    public async Task Delete(OldFeeding feeding)
    {
        var path = CreateDatabasePath(feeding.Started);

        var all = await GetFeedingsAsync(path);

        var temp = all.FirstOrDefault(x => x.Id == feeding.Id);
        if (temp is null)
        {
            return;
        }

        all.Remove(temp);
        await Save(all, path);
    }

    private static async Task Save(IEnumerable<OldFeeding> feedings, string path)
    {
        var json = JsonSerializer.Serialize(feedings);

        await File.WriteAllTextAsync(path, json);
    }

    private static async Task Save(List<OldFeeding> feedings)
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

    public List<OldFeeding> GetFeedings(string fileName)
    {
        if (!IsInit(fileName))
        {
            return [];
        }

        var jsonData = File.ReadAllText(fileName);

        if (jsonData == string.Empty)
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<OldFeeding>>(jsonData) ?? [];
    }

    public async Task<List<OldFeeding>> GetFeedingsAsync(string fileName)
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

        return JsonSerializer.Deserialize<List<OldFeeding>>(jsonData) ?? [];
    }

    public List<OldFeeding> GetAllFeedings()
    {
        List<OldFeeding> feedings = [];

            if (!Directory.Exists(DatabaseFolderPath))
            {
                return [];
            }
            var files = Directory.GetFiles(DatabaseFolderPath);
            foreach (var file in files)
            {
                feedings.AddRange(GetFeedings(file));
            }
        return feedings;
    }

    public async Task<List<OldFeeding>> GetFeedings(DateTime? start = null, DateTime? end = null)
    {
        List<OldFeeding> feedings = [];

        if (start is null)
        {
            start = DateTime.UtcNow.AddDays(-1);
        }

        if(start == DateTime.MinValue)
        {
            if(!Directory.Exists(DatabaseFolderPath))
            {
                return feedings;
            }
            var files = Directory.GetFiles(DatabaseFolderPath);
            foreach(var file in files)
            {
                feedings.AddRange(await GetFeedingsAsync(file));
            }
        }
        else
        {
            var dateIterator = start.Value.Date;

            while (dateIterator <= (end ?? DateTime.UtcNow).Date)
            {
                feedings.AddRange(await GetFeedingsAsync(CreateDatabasePath(dateIterator)));

                dateIterator = dateIterator.AddDays(1);
            }
        }

        return feedings
            .Where(x => x.Started >= (start ?? DateTime.MinValue) && x.Started <= (end ?? DateTime.UtcNow))
            .OrderByDescending(x => x.Started)
            .ToList();
    }

    public async Task<List<OldFeeding>> GetLast()
    {
        var all = await GetFeedings();
        return all.Take(2).ToList();
    }

    public async Task<bool> SaveFeeding(OldFeeding feeding)
    {
        var path = CreateDatabasePath(feeding.Started);
        var all = await GetFeedingsAsync(path);
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

    public void DeleteAll()
    {
        Directory.Delete(DatabaseFolderPath, true);
    }

    public Task DeleteAllAsync()
    {
        Directory.Delete(DatabaseFolderPath, true);
        return Task.FromResult(true);
    }

    public async Task<Settings> GetSettings()
    {
        if (!File.Exists(SettingsPath))
        {
            return new();
        }

        var data = await File.ReadAllTextAsync(SettingsPath);
        return JsonSerializer.Deserialize<Settings>(data) ?? new();
    }

    public async Task SaveSettings(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        await File.WriteAllTextAsync(SettingsPath, json);
    }
}

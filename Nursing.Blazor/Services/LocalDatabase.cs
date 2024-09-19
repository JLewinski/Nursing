using Blazored.LocalStorage;
using Nursing.Core.Models.DTO;

namespace Nursing.Blazor.Services;

internal class LocalDatabase
{
    private readonly ILocalStorageService _localStorageService;

    public LocalDatabase(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    private static string CreateDatabasePath(DateTime date)
    {
        return $"Data.{date:yyyy.MM.dd}";
    }

    public async Task Delete(FeedingDto feeding)
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

    private async Task Save(IEnumerable<FeedingDto> feedings, string storageKey)
    {
        await _localStorageService.SetItemAsync(storageKey, feedings);
    }

    private async Task Save(List<FeedingDto> feedings)
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

    public async Task<List<FeedingDto>> GetFeedingsAsync(string storageKey)
    {
        var data = await _localStorageService.GetItemAsync<List<FeedingDto>>(storageKey);
        return data ?? [];
    }

    public async Task<List<FeedingDto>> GetAllFeedings()
    {
        List<FeedingDto> feedings = [];

        var keys = await _localStorageService.KeysAsync();
        foreach (var storageKey in keys.Where(x => x.StartsWith("Data.")))
        {
            feedings.AddRange(await GetFeedingsAsync(storageKey));
        }
        return feedings;
    }

    public async Task<List<FeedingDto>> GetFeedings(DateTime? start = null, DateTime? end = null)
    {
        List<FeedingDto> feedings = [];

        start ??= DateTime.UtcNow;
        end ??= DateTime.UtcNow;

        if (start != DateTime.MinValue)
        {
            start = start.Value.Date.AddDays(-1);
        }
        if (end != DateTime.MaxValue)
        {
            end = end.Value.Date.AddDays(1);
        }

        if (start == DateTime.MinValue)
        {
            var keys = await _localStorageService.KeysAsync();
            foreach (var file in keys.Where(keys => keys.StartsWith("Data.")))
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

    public async Task<FeedingDto> GetLast()
    {
        var all = await GetFeedings();

        return all.Count == 0
            ? new FeedingDto
            {
                Id = Guid.Empty
            }
            : all[0];
    }

    public async Task<bool> SaveFeeding(FeedingDto feeding)
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

        all.Add(new(feeding));

        await Save(all, path);

        return true;
    }

    public async Task CleanData()
    {
        var feedings = await GetAllFeedings();
        var duplicates = feedings
            .GroupBy(x => x.Id)
            .Where(x => x.Count() > 1)
            .SelectMany(x => x.OrderByDescending(y => y.LastUpdated).Skip(1))
            .ToList();

        feedings.RemoveAll(duplicates.Contains);
        await Save(feedings);
    }

    public async Task<List<FeedingDto>> GetUpdatedFeedings(DateTime lastUpdated)
    {
        return await GetFeedings(lastUpdated);
    }

    public async Task SaveUpdated(List<FeedingDto> feedings)
    {
        foreach (var feeding in feedings)
        {
            await SaveFeeding(feeding);
        }
    }

    public async Task DeleteAll()
    {
        var keys = await _localStorageService.KeysAsync();
        foreach (var storageKey in keys.Where(x => x.StartsWith("Data.")))
        {
            await _localStorageService.RemoveItemAsync(storageKey);
        }
    }
}

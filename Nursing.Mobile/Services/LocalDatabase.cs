using Nursing.Core.Models.DTO;
using Nursing.Models;
using SQLite;

namespace Nursing.Mobile.Services;

internal class LocalDatabase : Nursing.Core.Services.IDatabase
{
    private const string DatabaseFilename = "Nursing.db3";

    public const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    private static string DatabasePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);

    private SQLiteAsyncConnection Database = null!;

    private async Task Init()
    {
        if (Database is not null)
        {
            return;
        }


        Database = new SQLiteAsyncConnection(DatabasePath, Flags);

        await Database.CreateTableAsync<FeedingDto>();

        CacheService cacheService = new();
        await cacheService.Cache(new(await GetLast()));
    }

    public async Task<FeedingDto> GetFeeding(Guid id)
    {
        await Init();
        return await Database.Table<FeedingDto>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> SaveFeeding(FeedingDto feeding)
    {
        await Init();
        int result;
        if (feeding.Id == Guid.Empty)
        {
            feeding.Id = Guid.NewGuid();
            result = await Database.InsertAsync(feeding.ToDto());
        }
        else
        {
            result = await Database.UpdateAsync(feeding.ToDto());
        }

        return result == 1;
    }

    private async Task Delete(Guid id)
    {
        await Init();
        await Database.Table<FeedingDto>().DeleteAsync(x => x.Id == id);
    }

    public async Task<List<FeedingDto>> GetFeedings(DateTime? start, DateTime? end)
    {
        await Init();
        start ??= DateTime.MinValue;
        end ??= DateTime.UtcNow;
        return await Database.Table<FeedingDto>()
            .Where(x => x.Started >= start && x.Started <= end)
            .OrderByDescending(x => x.Started)
            .ToListAsync();
    }

    public async Task<(TimeSpan averageTotal, TimeSpan averageRight, TimeSpan averageLeft, TimeSpan averageFinish)> GetAverages(DateTime? start, DateTime? end)
    {
        await Init();
        var data = await Database.Table<Feeding>()
            .Where(x => x.IsFinished && x.Finished != null && x.Started >= (start ?? DateTime.MinValue) && x.Started <= (end ?? DateTime.UtcNow))
            .ToListAsync();

        var (total, right, left, finish) = data.Aggregate((total: TimeSpan.Zero, right: TimeSpan.Zero, left: TimeSpan.Zero, finish: TimeSpan.Zero), (acc, x) =>
        {
            acc.total += x.TotalTime;
            acc.right += x.RightBreastTotal;
            acc.left += x.LeftBreastTotal;
            acc.finish += (x.Finished ?? DateTime.MaxValue) - x.Started;
            return acc;
        });

        return (
            TimeSpan.FromTicks(total.Ticks / data.Count),
            TimeSpan.FromTicks(right.Ticks / data.Count),
            TimeSpan.FromTicks(left.Ticks / data.Count),
            TimeSpan.FromTicks(finish.Ticks / data.Count));
    }

    public async Task<FeedingDto> GetLast()
    {
        await Init();

        return await Database.Table<FeedingDto>()
            .OrderByDescending(x => x.Started)
            .FirstOrDefaultAsync();
    }

    public Task Delete(FeedingDto feeding)
    {
        return Delete(feeding.Id);
    }

    public async Task DeleteAll()
    {
        await Init();
        await Database.Table<FeedingDto>().DeleteAsync();
    }
}

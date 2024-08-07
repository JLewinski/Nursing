using Nursing.Models;
using SQLite;

namespace Nursing.Mobile.Services;

internal class LocalDatabase// : IDatabase
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

        File.Delete(DatabasePath);

        Database = new SQLiteAsyncConnection(DatabasePath, Flags);

        await Database.CreateTableAsync<Feeding>();
    }

    public async Task<bool> SaveFeeding(Feeding feeding)
    {
        await Init();
        int result;
        if (feeding.Id == Guid.Empty)
        {
            feeding.Id = Guid.NewGuid();
            result = await Database.InsertAsync(feeding);
        }
        else
        {
            result = await Database.UpdateAsync(feeding);
        }

        return result == 1;
    }

    public async Task Delete(Guid id)
    {
        await Init();
        await Database.Table<Feeding>().DeleteAsync(x => x.Id == id);
    }

    public async Task<List<Feeding>> GetFeedings(DateTime? start, DateTime? end)
    {
        await Init();
        return await Database.Table<Feeding>()
            .Where(x => x.Finished >= (start ?? DateTime.MinValue) && x.Finished <= (end ?? DateTime.UtcNow))
            .OrderByDescending(x => x.Finished)
            .ToListAsync();
    }

    public async Task<(TimeSpan averageTotal, TimeSpan averageRight, TimeSpan averageLeft)> GetAverages(DateTime? start, DateTime? end)
    {
        await Init();
        var data = await Database.Table<Feeding>()
            .Where(x => x.Finished >= (start ?? DateTime.MinValue) && x.Finished <= (end ?? DateTime.UtcNow))
            .ToListAsync();

        var (total, right, left) = data.Aggregate((total: TimeSpan.Zero, right: TimeSpan.Zero, left: TimeSpan.Zero), (acc, x) =>
        {
            acc.total += x.RightBreastTotal + x.LeftBreastTotal;
            acc.right += x.RightBreastTotal;
            acc.left += x.LeftBreastTotal;
            return acc;
        });

        return (
            TimeSpan.FromTicks(total.Ticks / data.Count),
            TimeSpan.FromTicks(right.Ticks / data.Count),
            TimeSpan.FromTicks(left.Ticks / data.Count));
    }

    public async Task<List<Feeding>> GetLast()
    {
        try
        {
            await Init();
            return await Database.Table<Feeding>()
            .OrderByDescending(x => x.Finished)
            .Take(2)
            .ToListAsync();
        }
        catch (Exception)
        {
            return new List<Feeding>();
        }
    }
}

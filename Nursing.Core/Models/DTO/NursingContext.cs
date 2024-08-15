using Microsoft.EntityFrameworkCore;
using Nursing.Core.Models.DTO;

namespace Nursing.Models;

public class NursingContext : DbContext, Nursing.Core.Services.IDatabase
{

    public NursingContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<FeedingDto> Feedings { get; set; }

    public Task Delete(FeedingDto feeding)
    {
        Feedings.Remove(feeding);
        return SaveChangesAsync();
    }

    public Task DeleteAll()
    {
        Feedings.RemoveRange(Feedings);
        return SaveChangesAsync();
    }

    public async Task<(TimeSpan averageTotal, TimeSpan averageRight, TimeSpan averageLeft, TimeSpan averageFinish)> GetAverages(DateTime? start, DateTime? end)
    {
        var data = await GetFeedings(start, end);

        return data.Aggregate((total: TimeSpan.Zero, right: TimeSpan.Zero, left: TimeSpan.Zero, finish: TimeSpan.Zero), (acc, x) =>
        {
            acc.total += x.TotalTime;
            acc.right += x.RightBreastTotal;
            acc.left += x.LeftBreastTotal;
            acc.finish += (x.Finished ?? DateTime.MaxValue) - x.Started;
            return acc;
        });
    }

    public Task<FeedingDto> GetFeeding(Guid id)
    {
        return Feedings.FirstAsync(x => x.Id == id);
    }

    public Task<List<FeedingDto>> GetFeedings(DateTime? start, DateTime? end)
    {
        start ??= DateTime.MinValue;
        end ??= DateTime.UtcNow;

        return Feedings
            .Where(x => x.Started >= start && x.Started <= end)
            .OrderByDescending(x => x.Started)
            .ToListAsync();
    }

    public Task<FeedingDto> GetLast()
    {
        return Feedings.OrderByDescending(x => x.Started).FirstAsync();
    }

    public Task<bool> SaveFeeding(FeedingDto feeding)
    {
        if (feeding.Id == Guid.Empty)
        {
            feeding.Id = Guid.NewGuid();
            Feedings.Add(feeding);
        }
        else
        {
            Feedings.Update(feeding);
        }

        return SaveChangesAsync().ContinueWith(x => x.Result == 1);
    }
}
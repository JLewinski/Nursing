using Nursing.Core.Models.DTO;
using Nursing.Models;

namespace Nursing.Core.Services;

public interface IDatabase
{
    Task Delete(FeedingDto feeding);
    Task<(TimeSpan averageTotal, TimeSpan averageRight, TimeSpan averageLeft, TimeSpan averageFinish)> GetAverages(DateTime? start, DateTime? end);
    Task<FeedingDto> GetFeeding(Guid id);
    Task<List<FeedingDto>> GetFeedings(DateTime? start, DateTime? end);
    /// <summary>
    /// Gets the last feeding.
    /// </summary>
    /// <returns></returns>
    Task<FeedingDto> GetLast();
    Task<bool> SaveFeeding(FeedingDto feeding);
    Task DeleteAll();
}

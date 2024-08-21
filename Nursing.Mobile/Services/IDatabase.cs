using Nursing.Models;

namespace Nursing.Mobile.Services
{
    internal interface IDatabase
    {
        Task Delete(Guid id);
        Task<(TimeSpan averageTotal, TimeSpan averageRight, TimeSpan averageLeft)> GetAverages(DateTime? start, DateTime? end);
        Task<List<Feeding>> GetFeedings(DateTime? start, DateTime? end);
        Task<List<Feeding>> GetLast();
        Task<bool> SaveFeeding(Feeding feeding);
        Task<TimeSpan> GetInBetween();
    }
}
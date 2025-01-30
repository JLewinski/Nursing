// using Microsoft.EntityFrameworkCore;
// using Nursing.API.Models;
// using Nursing.Core.Models;

// namespace Nursing.API.Services;

// public interface ISyncService
// {
//     Task<SyncResult> SyncFeedings(SyncModel sync, string userId);
//     Task<int> DeleteFeedings(Guid[] ids, string userId);
// }

// public class SyncService : ISyncService
// {
//     private readonly PostgresContext _context;

//     public SyncService(PostgresContext context)
//     {
//         _context = context;
//     }

//     public async Task<SyncResult> SyncFeedings(SyncModel sync, string userId)
//     {
//         var currentUser = await _context.Users.FirstAsync(u => u.Id == userId);

//         var toSend = await _context.Feedings
//             .Where(f => f.LastUpdated > sync.LastSync && f.GroupId == currentUser.GroupId)
//             .Select(f => new FeedingDto(f))
//             .ToListAsync();

//         var ids = sync.Feedings.Select(x => x.Id).ToList();

//         var existing = await _context.Feedings
//             .Where(f => ids.Contains(f.Id))
//             .Select(x => new { x.Id, x.GroupId, x.Deleted })
//             .ToListAsync();

//         var existingIds = existing
//             .Where(x => x.GroupId == currentUser.GroupId && x.Deleted == null)
//             .Select(x => x.Id)
//             .ToList();

//         var badIds = existing
//             .Where(x => x.GroupId != currentUser.GroupId || x.Deleted != null)
//             .Select(x => x.Id)
//             .ToList();

//         var updateList = sync.Feedings
//             .Where(x => existingIds.Contains(x.Id))
//             .Select(x => new Feeding(x, currentUser.GroupId))
//             .ToList();

//         var insertList = sync.Feedings
//             .Where(x => !existingIds.Contains(x.Id) && !badIds.Contains(x.Id) && x.Deleted == null)
//             .Select(x => new Feeding(x, currentUser.GroupId))
//             .ToList();

//         badIds.AddRange(sync.Feedings
//             .Where(x => x.Deleted != null && !existingIds.Contains(x.Id))
//             .Select(x => x.Id)
//             .ToList());

//         _context.ChangeTracker.Clear();
//         _context.Feedings.UpdateRange(updateList);
//         _context.Feedings.AddRange(insertList);

//         var numChanged = await _context.SaveChangesAsync();

//         return new SyncResult { Success = true, Feedings = toSend, BadIds = badIds, Updates = numChanged };
//     }

//     public async Task<int> DeleteFeedings(Guid[] ids, string userId)
//     {
//         var currentUser = await _context.Users.FirstAsync(u => u.Id == userId);
//         return await _context.Feedings
//             .Where(f => ids.Contains(f.Id) && f.GroupId == currentUser.GroupId)
//             .ExecuteUpdateAsync(f => f.SetProperty(x => x.Deleted, DateTime.UtcNow));
//     }
// }

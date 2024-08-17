using Nursing.Core.Models.DTO;

namespace Nursing.Core.Models;
public class SyncResult
{
    public bool Success { get; set; }
    public required List<FeedingDto> Feedings { get; set; }
    public required List<Guid> BadIds { get; set; }
}

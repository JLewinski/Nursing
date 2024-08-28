using Nursing.Core.Models.DTO;

namespace Nursing.Core.Models;

public class SyncModel
{
    public DateTime LastSync { get; set; }
    public List<FeedingDto> Feedings { get; set; } = null!;
}

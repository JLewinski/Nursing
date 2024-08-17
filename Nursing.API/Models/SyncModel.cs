using Nursing.Core.Models.DTO;

namespace Nursing.API.Models;

public class SyncModel
{
    public DateTime LastSync { get; set; }
    public FeedingDto[] Feedings { get; set; } = null!;
}

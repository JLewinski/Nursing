using Nursing.API.Models;

namespace Nursing.Core.Models;

public class SyncModel
{
    public DateTime LastSync { get; set; }
    public List<Feeding> Feedings { get; set; } = null!;
}

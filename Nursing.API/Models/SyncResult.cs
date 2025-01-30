
using Nursing.API.Models;

namespace Nursing.Core.Models;
public class SyncResult
{
    public bool Success { get; set; }
    public required List<Feeding> Feedings { get; set; }
    public required List<Guid> BadIds { get; set; }
    public int Updates { get; set; }
}

namespace Nursing.API.Models;
public class SyncResult
{
    public bool Success { get; set; }
    public required List<Feeding> Feedings { get; set; }
    public required List<Guid> BadIds { get; set; }
}

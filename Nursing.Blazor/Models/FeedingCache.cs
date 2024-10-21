using Nursing.Core.Models.DTO;
using Nursing.Models;

namespace Nursing.Mobile.Models;

public class FeedingCache
{
    public FeedingCache()
    {
    }

    public FeedingCache(FeedingDto feeding)
    {
        if (feeding.Finished != null)
        {
            CurrentFeeding = new Feeding(feeding);
        }
        else
        {
            LastStart = feeding.Started;
            LastWasLeft = feeding.LastIsLeft;
        }
    }

    public Feeding CurrentFeeding { get; set; } = new();
    public DateTime? LastStart { get; set; }
    public bool? LastWasLeft { get; set; }
}

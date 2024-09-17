using Nursing.Core.Models.DTO;

namespace Nursing.Models;

public class Feeding : FeedingDto
{
    public Feeding()
    {

    }

    public Feeding(FeedingDto feeding)
    {
        Id = feeding.Id;
        LeftBreastTotal = feeding.LeftBreastTotal;
        RightBreastTotal = feeding.RightBreastTotal;
        TotalTime = feeding.TotalTime;
        Started = feeding.Started;
        Finished = feeding.Finished;
        LastIsLeft = feeding.LastIsLeft;
        LastUpdated = feeding.LastUpdated;
    }

    public List<FeedingTime> LeftBreast { get; init; } = [];

    public List<FeedingTime> RightBreast { get; init; } = [];

    public void StartLeftBreast()
    {
        var date = DateTime.UtcNow;

        if (LeftBreast.Count == 0 && RightBreast.Count == 0)
        {
            Started = date;
        }
        else if (LeftBreast.LastOrDefault() != null && !LeftBreast.Last().EndTime.HasValue)
        {
            return;
        }

        LeftBreast.Add(new FeedingTime { StartTime = date });
        EndRightBreast(date);
    }

    public void EndLeftBreast(DateTime? value = null)
    {
        if(LeftBreast.LastOrDefault() == null || LeftBreast.Last().EndTime.HasValue)
        {
            return;
        }
        LeftBreast.Last().EndTime = value ?? DateTime.UtcNow;
        Calculate();
    }

    public void StartRightBreast()
    {
        var date = DateTime.UtcNow;
        if (LeftBreast.Count == 0 && RightBreast.Count == 0)
        {
            Started = date;
        }
        else if (RightBreast.LastOrDefault() != null && !RightBreast.Last().EndTime.HasValue)
        {
            return;
        }
        RightBreast.Add(new FeedingTime { StartTime = date });
        EndLeftBreast(date);
    }

    public void EndRightBreast(DateTime? value = null)
    {
        if (RightBreast.LastOrDefault() == null || RightBreast.Last().EndTime.HasValue)
        {
            return;
        }
        RightBreast.Last().EndTime = value ?? DateTime.UtcNow;
        Calculate();
    }

    public void Calculate()
    {
        LeftBreastTotal = GetTotalTime(LeftBreast);
        RightBreastTotal = GetTotalTime(RightBreast);
        TotalTime = LeftBreastTotal + RightBreastTotal;
    }

    public void Finish()
    {
        var maxLeft = LeftBreast.Count > 0 ? LeftBreast.Max(x => x.StartTime) : DateTime.MinValue;
        var maxRight = RightBreast.Count > 0 ? RightBreast.Max(x => x.StartTime) : DateTime.MinValue;

        LastIsLeft = maxLeft > maxRight;

        Finished = DateTime.UtcNow;
        
        EndRightBreast(Finished);
        EndLeftBreast(Finished);
    }

    public static TimeSpan GetTotalTime(IEnumerable<FeedingTime> breast)
    {
        return breast.Aggregate(TimeSpan.Zero, (acc, x) => acc + ((x.EndTime ?? DateTime.UtcNow) - x.StartTime));
    }

    public bool IsFinished => Finished is not null;
}

public class FeedingTime
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class OldFeeding
{
    public Guid Id { get; set; }
    public List<FeedingTime> LeftBreast { get; set; } = [];
    public TimeSpan LeftBreastTotal { get; set; }
    public List<FeedingTime> RightBreast { get; set; } = [];
    public TimeSpan RightBreastTotal { get; set; }
    public TimeSpan TotalTime => LeftBreastTotal + RightBreastTotal;

    public void Calculate()
    {
        LeftBreastTotal = GetTotalTime(LeftBreast);
        RightBreastTotal = GetTotalTime(RightBreast);
    }

    public static TimeSpan GetTotalTime(IEnumerable<FeedingTime> breast)
    {
        return breast.Aggregate(TimeSpan.Zero, (acc, x) => acc + ((x.EndTime ?? DateTime.UtcNow) - x.StartTime));
    }

    public bool IsFinished { get; set; }

    public DateTime Started { get; set; }
    public DateTime Finished { get; set; }
}
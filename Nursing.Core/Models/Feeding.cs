using Nursing.Core.Models.DTO;

namespace Nursing.Models;

public class Feeding : FeedingDto
{
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
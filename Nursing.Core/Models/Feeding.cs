namespace Nursing.Models;

public class Feeding
{
    public Guid Id { get; set; }
    private List<FeedingTime> _leftBreast = [];
    public List<FeedingTime> LeftBreast
    {
        get => _leftBreast;
        init => _leftBreast = value.ToList();
    }
    private TimeSpan? _leftBreastTotal;
    public TimeSpan LeftBreastTotal
    {
        get
        {
            _leftBreastTotal ??= GetTotalTime(LeftBreast);
            return _leftBreastTotal.Value;
        }
        init
        {
            _leftBreastTotal = value;
        }
    }

    private List<FeedingTime> _rightBreast = [];
    public List<FeedingTime> RightBreast
    {
        get => _rightBreast;
        init => _rightBreast = value.ToList();
    }
    private TimeSpan? _rightBreastTotal;
    public TimeSpan RightBreastTotal
    {
        get
        {
            _rightBreastTotal ??= GetTotalTime(RightBreast);
            return _rightBreastTotal.Value;
        }
        init
        {
            _rightBreastTotal = value;
        }
    }

    public TimeSpan TotalTime => LeftBreastTotal + RightBreastTotal;

    public void StartLeftBreast()
    {
        _leftBreast.Add(new FeedingTime { StartTime = DateTime.UtcNow });
    }

    public void EndLeftBreast()
    {
        LastFinish = DateTime.UtcNow;
        _leftBreast.Last().EndTime = LastFinish;
        _leftBreastTotal = GetTotalTime(LeftBreast);
    }

    public void StartRightBreast()
    {
        _rightBreast.Add(new FeedingTime { StartTime = DateTime.UtcNow });
    }

    public void EndRightBreast()
    {
        LastFinish = DateTime.UtcNow;
        _rightBreast.Last().EndTime = LastFinish;
        _rightBreastTotal = GetTotalTime(RightBreast);
    }

    public TimeSpan GetTotalTime(IEnumerable<FeedingTime> breast)
    {
        return breast.Aggregate(TimeSpan.Zero, (acc, x) => acc + ((x.EndTime ?? DateTime.UtcNow) - x.StartTime));
    }

    public bool IsFinished { get; set; }

    public DateTime LastFinish { get; set; }
}

public class FeedingTime
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
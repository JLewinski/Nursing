namespace Nursing;

public class Feeding
{
    private List<FeedingTime> _leftBreast = [];
    public IReadOnlyCollection<FeedingTime> LeftBreast => _leftBreast;
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
    public IReadOnlyCollection<FeedingTime> RightBreast => _rightBreast;
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

    public void StartLeftBreast()
    {
        _leftBreast.Add(new FeedingTime { StartTime = DateTime.UtcNow });
    }

    public void EndLeftBreast()
    {
        _leftBreast.Last().EndTime = DateTime.UtcNow;
    }

    public void StartRightBreast()
    {
        _rightBreast.Add(new FeedingTime { StartTime = DateTime.UtcNow });
    }

    public void EndRightBreast()
    {
        _rightBreast.Last().EndTime = DateTime.UtcNow;
    }


    public TimeSpan GetTotalTime(IEnumerable<FeedingTime> breast)
    {
        return breast.Aggregate(TimeSpan.Zero, (acc, x) => acc + ((x.EndTime ?? DateTime.UtcNow) - x.StartTime));
    }

    public bool IsFinished { get; set; }


}

public class FeedingTime
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
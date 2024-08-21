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
        set
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
        set
        {
            _rightBreastTotal = value;
        }
    }

    public TimeSpan TotalTime => LeftBreastTotal + RightBreastTotal;

    public void StartLeftBreast()
    {
        var date = DateTime.UtcNow;

        if (_leftBreast.Count == 0 && _rightBreast.Count == 0)
        {
            Started = date;
        }
        else if (_leftBreast.LastOrDefault() != null && !_leftBreast.Last().EndTime.HasValue)
        {
            return;
        }

        _leftBreast.Add(new FeedingTime { StartTime = date });
        EndRightBreast(date);
    }

    public void EndLeftBreast(DateTime? value = null)
    {
        if(_leftBreast.LastOrDefault() == null || _leftBreast.Last().EndTime.HasValue)
        {
            return;
        }
        _leftBreast.Last().EndTime = value ?? DateTime.UtcNow;
        _leftBreastTotal = GetTotalTime(LeftBreast);
    }

    public void StartRightBreast()
    {
        var date = DateTime.UtcNow;
        if (_leftBreast.Count == 0 && _rightBreast.Count == 0)
        {
            Started = date;
        }
        else if (_rightBreast.LastOrDefault() != null && !_rightBreast.Last().EndTime.HasValue)
        {
            return;
        }
        _rightBreast.Add(new FeedingTime { StartTime = date });
        EndLeftBreast(date);
    }

    public void EndRightBreast(DateTime? value = null)
    {
        if (_rightBreast.LastOrDefault() == null || _rightBreast.Last().EndTime.HasValue)
        {
            return;
        }
        _rightBreast.Last().EndTime = value ?? DateTime.UtcNow;
        _rightBreastTotal = GetTotalTime(RightBreast);
    }

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

public class FeedingTime
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
namespace Nursing.API.Models;

public class Feeding
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;

    public TimeSpan LeftDuration { get; set; }
    public TimeSpan RightDuration { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? Deleted { get; set; }
}

public class Invite
{
    public Guid Id { get; set; }
    public string UserId1 { get; set; } = null!;
    public string UserId2 { get; set; } = null!;
    public bool Accepted { get; set; }
}
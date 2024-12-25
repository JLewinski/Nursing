using Nursing.Core.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace Nursing.API.Models;

public class Feeding : FeedingDto
{
    public Feeding()
    {
    }

    public Feeding(FeedingDto feeding, Guid groupId) : base(feeding)
    {
        GroupId = groupId;
    }
    public Guid GroupId { get; set; }
}

public class Invite
{
    [Key]
    public Guid GroupId { get; set; }
    public string UserId { get; set; } = null!;
}
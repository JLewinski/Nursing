using Microsoft.AspNetCore.Identity;

namespace Nursing.API.Models;

public class NursingUser
{
    public string Id { get; set; } = null!;
    public ICollection<Invite> Invites { get; set; } = [];
}

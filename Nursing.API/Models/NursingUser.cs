using Microsoft.AspNetCore.Identity;
using Nursing.Core.Models.DTO;

namespace Nursing.API.Models;

public class NursingUser : IdentityUser<Guid>
{
    public Guid GroupId { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }
}

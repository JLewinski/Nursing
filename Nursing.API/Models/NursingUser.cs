using Microsoft.AspNetCore.Identity;
using Nursing.Core.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nursing.API.Models;

public class NursingUser : IdentityUser<Guid>
{
    public Guid GroupId { get; set; }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;

namespace Nursing.API.Services;

public class SqlContext : IdentityDbContext<NursingUser, IdentityRole<Guid>, Guid>
{
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
    }

    public DbSet<Feeding> Feedings { get; set; }
    public DbSet<Invite> Invites { get; set; }
}

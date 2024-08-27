using AspNetCore.Identity.CosmosDb;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;

namespace Nursing.API.Services;

public class SqlContext : CosmosIdentityDbContext<NursingUser, IdentityRole<Guid>, Guid>
{
    public SqlContext(DbContextOptions<SqlContext> options) : base(options)
    {
    }

    public void Migrate()
    {
        Database.Migrate();
    }

    public DbSet<Feeding> Feedings { get; set; }
    public DbSet<Invite> Invites { get; set; }
}

using Microsoft.EntityFrameworkCore;

namespace Nursing.API.Models;

public interface INursingContext
{
    DbSet<Feeding> Feedings { get; set; }
    DbSet<Invite> Invites { get; set; }

    Task MigrateAsync();
}

public class NursingContext : DbContext, INursingContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=nursing.db");
    }

    public async Task MigrateAsync()
    {
        await Database.MigrateAsync();
    }

    public DbSet<Feeding> Feedings { get; set; } = null!;
    public DbSet<Invite> Invites { get; set; } = null!;
}
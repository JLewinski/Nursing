using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;
using Nursing.API.Services;

namespace Nursing.API.Tests.Utilities;

public static class TestDbContext
{
    public static PostgresContext Create()
    {
        var options = new DbContextOptionsBuilder<PostgresContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new PostgresContext(options);
        
        return context;
    }

    public static void Destroy(PostgresContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}

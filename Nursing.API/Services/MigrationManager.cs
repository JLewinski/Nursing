namespace Nursing.API.Services;

public static class MigrationManager
{
    public static async Task<WebApplication> EnsureCreated(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
            await context.Database.EnsureCreatedAsync();
        }
        return app;
    }
}

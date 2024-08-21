namespace Nursing.API.Services;

public static class MigrationManager
{
    public static WebApplication Migrate(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
            try
            {
                context.Migrate();
            }
            catch (Exception)
            {
            }
        }
        return app;
    }
}

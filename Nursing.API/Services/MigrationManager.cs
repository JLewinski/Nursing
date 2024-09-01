using Microsoft.AspNetCore.Identity;

namespace Nursing.API.Services;

public static class MigrationManager
{
    public static async Task<WebApplication> Migrate(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<SqlContext>();
            context.Migrate();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }
        }
        return app;
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;

namespace Nursing.API.Services;

public static class MigrationManager
{
    public static async Task<WebApplication> Migrate(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<SqlContext>();

        context.Migrate();

        var initialAdminConfig = app.Configuration.GetSection("InitialAdmin");
        var username = initialAdminConfig.GetValue<string>("User") ?? throw new NullReferenceException();
        var password = initialAdminConfig.GetValue<string>("Password") ?? throw new NullReferenceException();

        await AddUser(context, username, password, scope);

        return app;
    }

    private static async Task AddRole(IServiceScope scope)
    {
        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
        }
    }

    private static async Task AddUser(SqlContext context, string username, string password, IServiceScope scope)
    {
        if (await context.Users.AnyAsync(x => x.UserName == username))
        {
            return;
        }
        
        var user = new Models.NursingUser
        {
            UserName = username,
            Email = username,
            GroupId = Guid.NewGuid(),
            RefreshTokens = new()
        };

        using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<NursingUser>>();
        await userManager.CreateAsync(user, password);
    }
}

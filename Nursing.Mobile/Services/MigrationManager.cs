using Nursing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing.Mobile.Services;

public static class MigrationManager
{
    public static MauiApp MigrateDatabase(this MauiApp app)
    {
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<EFDatabase>();
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

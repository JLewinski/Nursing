using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nursing.Mobile.Services;
using Nursing.Services;

namespace Nursing.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });


        builder.Services.AddSingleton<HttpClient>();

        if(!Directory.Exists(CacheService.DataDirectory))
        {
            Directory.CreateDirectory(CacheService.DataDirectory);
        }
        
        var sqliteFilePath = Path.Combine(CacheService.DataDirectory, "Nursing.db");
        builder.Services.AddDbContext<EFDatabase>(options =>
            options.UseSqlite($"Data Source={sqliteFilePath}", opts =>
                opts.MigrationsAssembly("Nursing.Sqlite")));

        builder.Services.AddSingleton<CacheService>();
        builder.Services.AddSingleton<SyncService>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build().MigrateDatabase();
    }
}

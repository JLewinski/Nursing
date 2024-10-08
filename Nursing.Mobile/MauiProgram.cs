﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nursing.Core.Services;
using Nursing.Mobile.Services;
using Nursing.Services;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

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

        var sqliteFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Nursing.db");
        builder.Services.AddDbContext<IDatabase, EFDatabase>(options =>
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

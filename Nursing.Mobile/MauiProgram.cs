using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nursing.Core.Services;
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

        builder.Services.AddDbContext<EFDatabase>(options =>
			options.UseSqlite("temp.db", opts =>
				opts.MigrationsAssembly("Nursing.Sqlite")));

        builder.Services.AddSingleton<IDatabase, LocalDatabase>();
        builder.Services.AddSingleton<CacheService>();
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

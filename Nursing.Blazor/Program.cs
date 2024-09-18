using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Nursing.Blazor;
using Nursing.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddBlazoredToast();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddSingleton<LocalDatabase>();
builder.Services.AddSingleton<CacheService>();
builder.Services.AddSingleton<SyncService>();

await builder.Build().RunAsync();

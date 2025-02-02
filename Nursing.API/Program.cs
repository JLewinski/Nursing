using FastEndpoints;
using FastEndpoints.ClientGen.Kiota;
using FastEndpoints.Swagger;
using Nursing.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDbContext<INursingContext, NursingContext>();

builder.Services.AddAuthentication().AddJwtBearer(x =>
{
    x.Audience = builder.Configuration["Jwt:Audience"];
    x.Authority = builder.Configuration["Jwt:Authority"];
    x.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (!context.Request.Cookies.ContainsKey("CF_Authorization"))
            {
                return Task.CompletedTask;
            }

            var token = context.Request.Cookies["CF_Authorization"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "Nursing API";
        s.Description = "API for Nursing App";
        s.Version = "1.0";
        s.DocumentName = "v1";
    };
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(policy =>
    {
        policy.AddDefaultPolicy(builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseCors();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(options =>
{
    options.Endpoints.ShortNames = true;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
    app.UseSwaggerUI();
}

await app.ExportSwaggerJsonAndExitAsync("v1", "../Nursing.Svelte/", "api.json");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<INursingContext>();
    await db.MigrateAsync();
}

app.Run();
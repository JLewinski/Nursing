using FastEndpoints;
using FastEndpoints.Swagger;
using Nursing.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<INursingContext, NursingContext>();

builder.Services.AddAuthentication().AddJwtBearer(x =>
{
    x.Audience = builder.Configuration["Jwt:Audience"];
    x.Authority = builder.Configuration["Jwt:Authority"];
    x.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents{
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
builder.Services.SwaggerDocument(options => {
    options.DocumentSettings = s => {
        s.Title = "Nursing API";
        s.Description = "API for Nursing App";
        s.Version = "1.0";
        s.PostProcess = async d => {
            await File.WriteAllTextAsync("swagger.json", d.ToJson());
        };
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

app.UseFastEndpoints().UseSwaggerGen();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<INursingContext>();
    await db.Migrate();
}

app.Run();
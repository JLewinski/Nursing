using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nursing.API.Models;
using Nursing.API.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PostgresContext>(options =>
    options.UseSqlite("Data Source=nursing.db"));

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorizationBuilder();

builder.Services.AddIdentityCore<NursingUser>()
    .AddEntityFrameworkStores<PostgresContext>()
    .AddApiEndpoints();

builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IInviteService, InviteService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options => {
        options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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

var token = builder.Configuration.GetSection("Token").Get<TokenManagement>() ?? throw new InvalidOperationException("TokenManagement section is missing");

builder.Services
    .AddAuthentication();

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("Admin", p => p.RequireRole("Admin"));
});

var app = builder.Build();

await app.Migrate();

app.MapIdentityApi<NursingUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
    app.UseCors();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
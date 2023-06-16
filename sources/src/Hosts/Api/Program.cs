using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddApiVersioning();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });
        builder.Services.AddUserPostgreStorage(builder.Configuration);
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // адрес сервера SecurityService
                options.Authority = builder.Configuration.GetSection("IdentityServerClient:Authority").Value;

                options.Audience = builder.Configuration.GetSection("IdentityServerClient:Audience").Value;

                // пока сервер не поддерживает https
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30);
            });

        var app = builder.Build();
        if (args.Length > 0 && args[0] == "update")
        {
            await UpdateDb(app);
            return;
        }

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        // app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAuthorization();

        // app.MapMetrics();
        app.MapControllers();

        app.Run();
    }

    private static async Task UpdateDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await db.Database.MigrateAsync();
    }
}
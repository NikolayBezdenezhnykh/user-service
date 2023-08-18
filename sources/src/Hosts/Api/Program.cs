using Application.Implementations;
using Infrastructure;
using Infrastructure.KafkaConsumerHandlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddUserPostgreStorage(builder.Configuration);
        if (args.Length > 0 && args[0] == "update")
        {
            await UpdateDb(builder.Build());
            return;
        }

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddApiVersioning();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped<KafkaMessageCreateUserHandler>();
        builder.Services.AddSingleton<IKafkaMessageHandlerFactory, KafkaMessageHandlerFactory>();
        builder.Services.AddHostedService<KafkaConsumerHandler>();
        builder.Services.Configure<KafkaConsumerConfig>(options => builder.Configuration.GetSection("KafkaConsumer").Bind(options));
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
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // адрес сервера auth-service
                options.Authority = builder.Configuration.GetSection("IdentityServerClient:Authority").Value;

                options.Audience = builder.Configuration.GetSection("IdentityServerClient:Audience").Value;

                // сервер не поддерживает https
                options.RequireHttpsMetadata = false;

                if (builder.Environment.IsDevelopment())
                {
                    options.TokenValidationParameters.ValidateIssuer = false;
                }

                options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30);
            });

        var app = builder.Build();       

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
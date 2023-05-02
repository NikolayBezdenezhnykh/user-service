using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddUserPostgreStorage(builder.Configuration);
        
        var app = builder.Build();
        if (args.Length > 0 && args[0] == "update")
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            await db.Database.MigrateAsync();
            return;
        }

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpMetrics();
        app.UseAuthorization();

        app.MapMetrics();
        app.MapControllers();

        app.Run();
    }
}
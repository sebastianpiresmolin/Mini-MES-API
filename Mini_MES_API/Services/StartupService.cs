using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mini_MES_API.Data;

namespace Mini_MES_API.Services;

public class StartupService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupService> _logger;

    public StartupService(
        IServiceProvider serviceProvider, 
        ILogger<StartupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        await context.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Migrations applied.");

        await SeedDatabaseAsync(context);
    }

    private async Task SeedDatabaseAsync(DataContext context)
    {
        try
        {
            var sqlFile = Path.Combine("Data", "seed.sql");
            if (File.Exists(sqlFile))
            {
                var sqlScript = await File.ReadAllTextAsync(sqlFile);
                await context.Database.ExecuteSqlRawAsync(sqlScript);
                _logger.LogInformation("Database seeded.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}


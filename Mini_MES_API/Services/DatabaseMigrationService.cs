using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mini_MES_API.Data;

namespace Mini_MES_API.Services
{
    public class DatabaseMigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseMigrationService> _logger;

        public DatabaseMigrationService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseMigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            _logger.LogInformation("Applying database migrations...");
            
            int retries = 0;
            const int maxRetries = 10;
            
            while (retries < maxRetries)
            {
                try
                {
                    await context.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Database migrations applied successfully");
                    break;
                }
                catch (Exception ex)
                {
                    retries++;
                    _logger.LogWarning($"Database migration failed (Attempt {retries}/{maxRetries}): {ex.Message}");
                    
                    if (retries >= maxRetries)
                    {
                        _logger.LogError("Failed to apply migrations after multiple attempts");
                        throw;
                    }
                    
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
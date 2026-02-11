namespace MedManagerApi.BackgroundServices;

public class SearchLogCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SearchLogCleanupBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);
    private readonly int _daysToKeep = 90;

    public SearchLogCleanupBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<SearchLogCleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Search Log Cleanup Background Service is starting");

        // Wait 1 minute after startup before first cleanup
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Search Log Cleanup Background Service is running cleanup at: {Time}", 
                    DateTimeOffset.UtcNow);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var cleanupService = scope.ServiceProvider
                        .GetRequiredService<Services.ISearchLogCleanupService>();

                    // Get current stats before cleanup
                    var beforeCount = await cleanupService.GetLogCountAsync();
                    var beforeSize = await cleanupService.GetLogSizeEstimateAsync();

                    _logger.LogInformation(
                        "Before cleanup - Total logs: {Count}, Estimated size: {Size:N0} bytes ({SizeMB:F2} MB)",
                        beforeCount, beforeSize, beforeSize / 1024.0 / 1024.0);

                    // Perform cleanup
                    await cleanupService.CleanupOldLogsAsync(_daysToKeep);

                    // Get stats after cleanup
                    var afterCount = await cleanupService.GetLogCountAsync();
                    var afterSize = await cleanupService.GetLogSizeEstimateAsync();

                    _logger.LogInformation(
                        "After cleanup - Total logs: {Count}, Estimated size: {Size:N0} bytes ({SizeMB:F2} MB)",
                        afterCount, afterSize, afterSize / 1024.0 / 1024.0);

                    var deletedCount = beforeCount - afterCount;
                    var freedSize = beforeSize - afterSize;

                    _logger.LogInformation(
                        "Cleanup summary - Deleted: {Deleted} logs, Freed: {Freed:N0} bytes ({FreedMB:F2} MB)",
                        deletedCount, freedSize, freedSize / 1024.0 / 1024.0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during search log cleanup");
            }

            // Wait for the next interval
            _logger.LogInformation("Next cleanup scheduled in {Hours} hours", _checkInterval.TotalHours);
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Search Log Cleanup Background Service is stopping");
    }
}

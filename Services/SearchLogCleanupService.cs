using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;

namespace MedManagerApi.Services;

public class SearchLogCleanupService : ISearchLogCleanupService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<SearchLogCleanupService> _logger;

    public SearchLogCleanupService(MedManagerDbContext context, ILogger<SearchLogCleanupService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Removes search logs older than specified days
    /// </summary>
    /// <param name="daysToKeep">Number of days to keep logs (default: 90 days)</param>
    public async Task CleanupOldLogsAsync(int daysToKeep = 90)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            
            _logger.LogInformation("Starting cleanup of search logs older than {CutoffDate}", cutoffDate);

            // Delete logs in batches to avoid locking the database for too long
            int batchSize = 1000;
            int totalDeleted = 0;
            
            while (true)
            {
                var logsToDelete = await _context.SearchLogs
                    .Where(sl => sl.SearchedAt < cutoffDate)
                    .Take(batchSize)
                    .ToListAsync();

                if (!logsToDelete.Any())
                    break;

                _context.SearchLogs.RemoveRange(logsToDelete);
                await _context.SaveChangesAsync();
                
                totalDeleted += logsToDelete.Count;
                
                _logger.LogInformation("Deleted {Count} search logs (Total: {Total})", 
                    logsToDelete.Count, totalDeleted);

                // Small delay to prevent overwhelming the database
                await Task.Delay(100);
            }

            _logger.LogInformation("Cleanup completed. Total deleted: {TotalDeleted} logs", totalDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during search log cleanup");
            throw;
        }
    }

    /// <summary>
    /// Gets the total count of search logs
    /// </summary>
    public async Task<int> GetLogCountAsync()
    {
        return await _context.SearchLogs.CountAsync();
    }

    /// <summary>
    /// Estimates the size of search logs in bytes
    /// (Rough estimate: ~500 bytes per log entry)
    /// </summary>
    public async Task<long> GetLogSizeEstimateAsync()
    {
        var count = await GetLogCountAsync();
        // Rough estimate: each log entry is approximately 500 bytes
        return count * 500L;
    }
}

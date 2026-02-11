namespace MedManagerApi.Services;

public interface ISearchLogCleanupService
{
    Task CleanupOldLogsAsync(int daysToKeep = 90);
    Task<int> GetLogCountAsync();
    Task<long> GetLogSizeEstimateAsync();
}

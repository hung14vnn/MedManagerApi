using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.SuperAdmin)]
public class SearchAnalyticsController : ControllerBase
{
    private readonly ISearchLogService _searchLogService;
    private readonly ISearchLogCleanupService _searchLogCleanupService;

    public SearchAnalyticsController(
        ISearchLogService searchLogService, 
        ISearchLogCleanupService searchLogCleanupService)
    {
        _searchLogService = searchLogService;
        _searchLogCleanupService = searchLogCleanupService;
    }

    /// <summary>
    /// Get recent searches (last 50)
    /// </summary>
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentSearches([FromQuery] int count = 50)
    {
        var searches = await _searchLogService.GetRecentSearchesAsync(count);
        return Ok(new
        {
            total = searches.Count,
            searches = searches.Select(s => new
            {
                s.Id,
                s.SearchQuery,
                s.EntityType,
                s.ResultCount,
                s.FoundResults,
                s.UserId,
                userName = s.User?.Email,
                s.IpAddress,
                s.SearchedAt
            })
        });
    }

    /// <summary>
    /// Get popular searches in the last N days
    /// </summary>
    [HttpGet("popular")]
    public async Task<IActionResult> GetPopularSearches(
        [FromQuery] SearchEntityType? entityType = null,
        [FromQuery] int days = 7,
        [FromQuery] int top = 10)
    {
        var popularSearches = await _searchLogService.GetPopularSearchesAsync(entityType, days, top);
        return Ok(new
        {
            period = $"Last {days} days",
            entityType = entityType?.ToString() ?? "All",
            top,
            searches = popularSearches.Select(kvp => new
            {
                query = kvp.Key,
                count = kvp.Value
            })
        });
    }

    /// <summary>
    /// Get search statistics by entity type
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetSearchStats([FromQuery] int days = 30)
    {
        var stats = await _searchLogService.GetSearchStatsByEntityTypeAsync(days);
        return Ok(new
        {
            period = $"Last {days} days",
            totalSearches = stats.Values.Sum(),
            byEntityType = stats.Select(kvp => new
            {
                entityType = kvp.Key,
                count = kvp.Value,
                percentage = stats.Values.Sum() > 0 ? Math.Round((double)kvp.Value / stats.Values.Sum() * 100, 2) : 0
            })
        });
    }

    /// <summary>
    /// Get user search history
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserSearchHistory(string userId, [FromQuery] int count = 50)
    {
        var searches = await _searchLogService.GetUserSearchHistoryAsync(userId, count);
        return Ok(new
        {
            userId,
            total = searches.Count,
            searches = searches.Select(s => new
            {
                s.Id,
                s.SearchQuery,
                s.EntityType,
                s.ResultCount,
                s.FoundResults,
                s.SearchedAt
            })
        });
    }

    /// <summary>
    /// Get search log statistics (count and size)
    /// </summary>
    [HttpGet("info")]
    public async Task<IActionResult> GetLogInfo()
    {
        var count = await _searchLogCleanupService.GetLogCountAsync();
        var size = await _searchLogCleanupService.GetLogSizeEstimateAsync();
        
        return Ok(new
        {
            totalLogs = count,
            estimatedSizeBytes = size,
            estimatedSizeMB = Math.Round(size / 1024.0 / 1024.0, 2),
            message = "Size is an estimate based on ~500 bytes per log entry"
        });
    }

    /// <summary>
    /// Manually trigger cleanup of old search logs
    /// </summary>
    [HttpPost("cleanup")]
    public async Task<IActionResult> CleanupLogs([FromQuery] int daysToKeep = 90)
    {
        if (daysToKeep < 30)
            return BadRequest(new { message = "For safety, daysToKeep should be at least 30 days" });

        var beforeCount = await _searchLogCleanupService.GetLogCountAsync();
        var beforeSize = await _searchLogCleanupService.GetLogSizeEstimateAsync();

        await _searchLogCleanupService.CleanupOldLogsAsync(daysToKeep);

        var afterCount = await _searchLogCleanupService.GetLogCountAsync();
        var afterSize = await _searchLogCleanupService.GetLogSizeEstimateAsync();

        var deletedCount = beforeCount - afterCount;
        var freedSize = beforeSize - afterSize;

        return Ok(new
        {
            before = new
            {
                totalLogs = beforeCount,
                sizeBytes = beforeSize,
                sizeMB = Math.Round(beforeSize / 1024.0 / 1024.0, 2)
            },
            after = new
            {
                totalLogs = afterCount,
                sizeBytes = afterSize,
                sizeMB = Math.Round(afterSize / 1024.0 / 1024.0, 2)
            },
            deleted = new
            {
                logs = deletedCount,
                freedBytes = freedSize,
                freedMB = Math.Round(freedSize / 1024.0 / 1024.0, 2)
            },
            message = $"Successfully deleted {deletedCount} search logs older than {daysToKeep} days"
        });
    }
}


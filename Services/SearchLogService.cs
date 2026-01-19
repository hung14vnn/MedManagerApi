using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class SearchLogService : ISearchLogService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<SearchLogService> _logger;

    public SearchLogService(MedManagerDbContext context, ILogger<SearchLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogSearchAsync(
        string searchQuery, 
        SearchEntityType entityType, 
        int resultCount, 
        string? userId = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        try
        {
            var searchLog = new SearchLog
            {
                SearchQuery = searchQuery,
                EntityType = entityType,
                ResultCount = resultCount,
                FoundResults = resultCount > 0,
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                SearchedAt = DateTime.UtcNow
            };

            _context.SearchLogs.Add(searchLog);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log search: {Query}", searchQuery);
            // Don't throw - search logging shouldn't break the search functionality
        }
    }

    public async Task<List<SearchLog>> GetRecentSearchesAsync(int count = 50)
    {
        return await _context.SearchLogs
            .Include(s => s.User)
            .OrderByDescending(s => s.SearchedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<SearchLog>> GetUserSearchHistoryAsync(string userId, int count = 50)
    {
        return await _context.SearchLogs
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SearchedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetPopularSearchesAsync(
        SearchEntityType? entityType = null, 
        int days = 7, 
        int top = 10)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        
        var query = _context.SearchLogs
            .Where(s => s.SearchedAt >= cutoffDate);

        if (entityType.HasValue)
        {
            query = query.Where(s => s.EntityType == entityType.Value);
        }

        return await query
            .GroupBy(s => s.SearchQuery.ToLower())
            .Select(g => new { Query = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToDictionaryAsync(x => x.Query, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetSearchStatsByEntityTypeAsync(int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);
        
        return await _context.SearchLogs
            .Where(s => s.SearchedAt >= cutoffDate)
            .GroupBy(s => s.EntityType)
            .Select(g => new { EntityType = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.EntityType, x => x.Count);
    }
}

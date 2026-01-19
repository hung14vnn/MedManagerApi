using MedManagerApi.Models;

namespace MedManagerApi.Services;

public interface ISearchLogService
{
    Task LogSearchAsync(
        string searchQuery, 
        SearchEntityType entityType, 
        int resultCount, 
        string? userId = null,
        string? ipAddress = null,
        string? userAgent = null);
    
    Task<List<SearchLog>> GetRecentSearchesAsync(int count = 50);
    Task<List<SearchLog>> GetUserSearchHistoryAsync(string userId, int count = 50);
    Task<Dictionary<string, int>> GetPopularSearchesAsync(SearchEntityType? entityType = null, int days = 7, int top = 10);
    Task<Dictionary<string, int>> GetSearchStatsByEntityTypeAsync(int days = 30);
}

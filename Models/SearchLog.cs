namespace MedManagerApi.Models;

public enum SearchEntityType
{
    Drug,
    Ingredient,
    Disease,
    Interaction
}

public class SearchLog
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    public string SearchQuery { get; set; } = string.Empty;
    public SearchEntityType EntityType { get; set; }
    public int ResultCount { get; set; }
    public bool FoundResults { get; set; }
    
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
}

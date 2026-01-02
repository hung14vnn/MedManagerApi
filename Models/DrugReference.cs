namespace MedManagerApi.Models;

public class DrugReference
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string? Authors { get; set; }
    public string? Source { get; set; }
    public string? Url { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? Doi { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

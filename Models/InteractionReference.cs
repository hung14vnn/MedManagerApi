namespace MedManagerApi.Models;

public class InteractionReference
{
    public int Id { get; set; }
    public int InteractionId { get; set; }
    public DrugInteraction Interaction { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string? Authors { get; set; }
    public string? Source { get; set; }
    public string? Url { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? Doi { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

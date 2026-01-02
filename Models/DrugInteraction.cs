namespace MedManagerApi.Models;

public enum InteractionSeverity
{
    Mild,
    Moderate,
    Severe
}

public class DrugInteraction
{
    public int Id { get; set; }
    
    public int Drug1Id { get; set; }
    public Drug Drug1 { get; set; } = null!;
    
    public int Drug2Id { get; set; }
    public Drug Drug2 { get; set; } = null!;
    
    public InteractionSeverity Severity { get; set; }
    public string Mechanism { get; set; } = string.Empty;
    public string ClinicalEffects { get; set; } = string.Empty;
    public string ManagementRecommendations { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public ICollection<InteractionReference> References { get; set; } = new List<InteractionReference>();
}

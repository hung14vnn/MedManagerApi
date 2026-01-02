namespace MedManagerApi.Models;

public class DiseaseProtocol
{
    public int Id { get; set; }
    
    public int DiseaseId { get; set; }
    public Disease Disease { get; set; } = null!;
    
    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;
    
    public bool IsPreferred { get; set; }
    public int PreferenceOrder { get; set; }
    public string? DosageRecommendation { get; set; }
    public string? SpecialConditions { get; set; }
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

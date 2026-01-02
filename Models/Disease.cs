namespace MedManagerApi.Models;

public class Disease
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IcdCode { get; set; }
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public ICollection<DiseaseProtocol> TreatmentProtocols { get; set; } = new List<DiseaseProtocol>();
}

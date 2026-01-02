namespace MedManagerApi.Models;

public class Drug
{
    public int Id { get; set; }
    public string ActiveIngredient { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string? PharmacologicalGroup { get; set; }
    
    // Indications & Contraindications
    public string? Indications { get; set; }
    public string? Contraindications { get; set; }
    
    // Dosage Information
    public string? DosageAdults { get; set; }
    public string? DosageChildren { get; set; }
    public string? DosageHepaticImpairment { get; set; }
    public string? DosageRenalImpairment { get; set; }
    
    // Adverse Effects
    public string? AdverseEffects { get; set; }
    
    // Special Precautions
    public string? PregnancyPrecautions { get; set; }
    public string? BreastfeedingPrecautions { get; set; }
    public string? OtherPrecautions { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public ICollection<DrugReference> References { get; set; } = new List<DrugReference>();
    public ICollection<DrugInteraction> InteractionsAsDrug1 { get; set; } = new List<DrugInteraction>();
    public ICollection<DrugInteraction> InteractionsAsDrug2 { get; set; } = new List<DrugInteraction>();
    public ICollection<DiseaseProtocol> DiseaseProtocols { get; set; } = new List<DiseaseProtocol>();
}

namespace MedManagerApi.Models;

public enum DrugStatus
{
    Active,
    Inactive,
    Deprecated
}

public class Drug
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Status field
    public DrugStatus Status { get; set; } = DrugStatus.Active;

    // Foreign Keys for Dosage Form and Route
    public int? DosageFormId { get; set; }
    public DosageForm? DosageForm { get; set; }

    public int? RouteId { get; set; }
    public RouteInformation? Route { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<DrugIngredient> DrugIngredients { get; set; } = new List<DrugIngredient>();
        public ICollection<DrugReference> References { get; set; } = new List<DrugReference>();
        public ICollection<DrugInteraction> InteractionsAsDrug1 { get; set; } = new List<DrugInteraction>();
        public ICollection<DrugInteraction> InteractionsAsDrug2 { get; set; } = new List<DrugInteraction>();
        public ICollection<DiseaseProtocol> DiseaseProtocols { get; set; } = new List<DiseaseProtocol>();
    }

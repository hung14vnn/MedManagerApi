namespace MedManagerApi.Models;

public class MechanismInformation
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public ICollection<IngredientMechanism> IngredientMechanisms { get; set; } = new List<IngredientMechanism>();
}

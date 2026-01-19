namespace MedManagerApi.Models;

public class IngredientMechanism
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    
    public int MechanismId { get; set; }
    public MechanismInformation Mechanism { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

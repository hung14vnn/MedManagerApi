namespace MedManagerApi.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public ICollection<DrugIngredient> DrugIngredients { get; set; } = new List<DrugIngredient>();
    public ICollection<IngredientMechanism> IngredientMechanisms { get; set; } = new List<IngredientMechanism>();
}

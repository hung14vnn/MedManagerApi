namespace MedManagerApi.Models;

public class DrugIngredient
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;
    
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    
    public string? Strength { get; set; }
    public string? Unit { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

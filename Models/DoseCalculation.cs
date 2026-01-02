namespace MedManagerApi.Models;

public class DoseCalculation
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;
    
    public string CalculationType { get; set; } = string.Empty; // e.g., "BodyWeight", "CrCl", "eGFR"
    public string Formula { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public decimal? MinDose { get; set; }
    public decimal? MaxDose { get; set; }
    public string? Instructions { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

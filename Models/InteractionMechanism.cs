namespace MedManagerApi.Models;

public class InteractionMechanism
{
    public int Id { get; set; }
    public int InteractionId { get; set; }
    public DrugInteraction Interaction { get; set; } = null!;
    
    public int MechanismId { get; set; }
    public MechanismInformation Mechanism { get; set; } = null!;
    
    public string? MechanismType { get; set; } // "pharmacodynamic" or "pharmacokinetic"
    public string? InteractionText { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

namespace MedManagerApi.Models;

public class CounselingChecklist
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;
    
    public string CheckpointCategory { get; set; } = string.Empty; // e.g., "Administration", "Storage", "Side Effects"
    public string CheckpointText { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

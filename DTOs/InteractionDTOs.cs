namespace MedManagerApi.DTOs;

public record InteractionCheckRequest(List<int> DrugIds);

public record InteractionCheckResponse(
    List<InteractionDetailDto> Interactions,
    string OverallSeverity
);

public record InteractionDetailDto(
    int Id,
    DrugSearchDto Drug1,
    DrugSearchDto Drug2,
    string Severity,
    string Mechanism,
    string ClinicalEffects,
    string ManagementRecommendations,
    List<ReferenceDto> References
);

public record CreateInteractionDto(
    int Drug1Id,
    int Drug2Id,
    string Severity,
    string Mechanism,
    string ClinicalEffects,
    string ManagementRecommendations
);

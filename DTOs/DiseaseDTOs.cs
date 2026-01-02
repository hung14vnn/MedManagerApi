namespace MedManagerApi.DTOs;

public record DiseaseDto(
    int Id,
    string Name,
    string? IcdCode,
    string? Description
);

public record CreateDiseaseDto(
    string Name,
    string? IcdCode,
    string? Description
);

public record DiseaseProtocolDto(
    int Id,
    DiseaseDto Disease,
    DrugSearchDto Drug,
    bool IsPreferred,
    int PreferenceOrder,
    string? DosageRecommendation,
    string? SpecialConditions,
    string? Notes
);

public record DiseaseTreatmentDto(
    DiseaseDto Disease,
    List<TreatmentOptionDto> PreferredDrugs,
    List<TreatmentOptionDto> AlternativeDrugs
);

public record TreatmentOptionDto(
    DrugSearchDto Drug,
    string? DosageRecommendation,
    string? SpecialConditions,
    string? Notes
);

public record CreateProtocolDto(
    int DiseaseId,
    int DrugId,
    bool IsPreferred,
    int PreferenceOrder,
    string? DosageRecommendation,
    string? SpecialConditions,
    string? Notes
);

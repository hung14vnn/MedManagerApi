namespace MedManagerApi.DTOs;

public record DoseCalculationRequest(
    int DrugId,
    decimal? BodyWeight,
    decimal? CreatinineClearance,
    decimal? eGFR
);

public record DoseCalculationResponse(
    DrugSearchDto Drug,
    decimal CalculatedDose,
    string Unit,
    string CalculationType,
    string Instructions,
    List<string> Warnings
);

public record CreateDoseCalculationDto(
    int DrugId,
    string CalculationType,
    string Formula,
    string? Unit,
    decimal? MinDose,
    decimal? MaxDose,
    string? Instructions
);

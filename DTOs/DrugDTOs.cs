namespace MedManagerApi.DTOs;

public record DrugSearchDto(
    int Id,
    string ActiveIngredient,
    string BrandName,
    string? PharmacologicalGroup
);

public record DrugDetailDto(
    int Id,
    string ActiveIngredient,
    string BrandName,
    string? PharmacologicalGroup,
    string? Indications,
    string? Contraindications,
    string? DosageAdults,
    string? DosageChildren,
    string? DosageHepaticImpairment,
    string? DosageRenalImpairment,
    string? AdverseEffects,
    string? PregnancyPrecautions,
    string? BreastfeedingPrecautions,
    string? OtherPrecautions,
    List<ReferenceDto> References
);

public record CreateDrugDto(
    string ActiveIngredient,
    string BrandName,
    string? PharmacologicalGroup,
    string? Indications,
    string? Contraindications,
    string? DosageAdults,
    string? DosageChildren,
    string? DosageHepaticImpairment,
    string? DosageRenalImpairment,
    string? AdverseEffects,
    string? PregnancyPrecautions,
    string? BreastfeedingPrecautions,
    string? OtherPrecautions
);

public record ReferenceDto(
    int Id,
    string Title,
    string? Authors,
    string? Source,
    string? Url,
    DateTime? PublicationDate,
    string? Doi
);

public record CreateReferenceDto(
    string Title,
    string? Authors,
    string? Source,
    string? Url,
    DateTime? PublicationDate,
    string? Doi
);

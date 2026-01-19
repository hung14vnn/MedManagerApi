using MedManagerApi.Models;

namespace MedManagerApi.DTOs;

public record DrugSearchDto(
    int Id,
    string Code,
    string Name,
    string Status,
    string? DosageForm,
    string? Route
);

public record DrugDetailDto(
    int Id,
    string Code,
    string Name,
    string Status,
    DosageFormDto? DosageForm,
    RouteDto? Route,
    List<DrugIngredientDto> Ingredients,
    List<ReferenceDto> References,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateDrugDto(
    string Code,
    string Name,
    string Status,
    int? DosageFormId,
    int? RouteId,
    List<CreateDrugIngredientDto> Ingredients
);

public record UpdateDrugDto(
    string Code,
    string Name,
    string Status,
    int? DosageFormId,
    int? RouteId,
    List<CreateDrugIngredientDto> Ingredients
);

public record DrugIngredientDto(
    int Id,
    IngredientDto Ingredient,
    string? Strength,
    string? Unit
);

public record CreateDrugIngredientDto(
    int IngredientId,
    string? Strength,
    string? Unit
);

public record IngredientDto(
    int Id,
    string Code,
    string Name
);

public record DosageFormDto(
    int Id,
    string Code,
    string Name
);

public record RouteDto(
    int Id,
    string Code,
    string Name
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

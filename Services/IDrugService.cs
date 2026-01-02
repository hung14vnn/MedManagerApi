using MedManagerApi.DTOs;

namespace MedManagerApi.Services;

public interface IDrugService
{
    Task<List<DrugSearchDto>> SearchDrugsAsync(string? searchTerm = null);
    Task<DrugDetailDto?> GetDrugByIdAsync(int id);
    Task<DrugDetailDto> CreateDrugAsync(CreateDrugDto dto);
    Task<DrugDetailDto?> UpdateDrugAsync(int id, CreateDrugDto dto);
    Task<bool> DeleteDrugAsync(int id);
    Task<ReferenceDto> AddReferenceAsync(int drugId, CreateReferenceDto dto);
}

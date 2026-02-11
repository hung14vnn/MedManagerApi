using MedManagerApi.DTOs;

namespace MedManagerApi.Services;

public interface IDrugService
{
    Task<(List<DrugSearchDto> drugs, int totalCount, int totalPages)> GetAllDrugsAsync(int page, int pageSize);
    Task<List<DrugSearchDto>> SearchDrugsAsync(string? searchTerm = null);
    Task<DrugDetailDto?> GetDrugByIdAsync(int id);
    Task<DrugDetailDto> CreateDrugAsync(CreateDrugDto dto);
    Task<DrugDetailDto?> UpdateDrugAsync(int id, UpdateDrugDto dto);
    Task<bool> DeleteDrugAsync(int id);
    Task<ReferenceDto> AddReferenceAsync(int drugId, CreateReferenceDto dto);
}

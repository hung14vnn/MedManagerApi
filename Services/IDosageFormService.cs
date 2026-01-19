using MedManagerApi.Models;

namespace MedManagerApi.Services;

public interface IDosageFormService
{
    Task<List<DosageForm>> GetAllAsync();
    Task<DosageForm?> GetByIdAsync(int id);
    Task<(bool success, string message, int? id)> CreateAsync(DosageForm dosageForm);
    Task<(bool success, string message)> UpdateAsync(int id, DosageForm dosageForm);
    Task<(bool success, string message)> DeleteAsync(int id);
}

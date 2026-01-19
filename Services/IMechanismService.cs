using MedManagerApi.Models;

namespace MedManagerApi.Services;

public interface IMechanismService
{
    Task<List<MechanismInformation>> GetAllAsync();
    Task<MechanismInformation?> GetByIdAsync(int id);
    Task<(bool success, string message, int? id)> CreateAsync(MechanismInformation mechanism);
    Task<(bool success, string message)> UpdateAsync(int id, MechanismInformation mechanism);
    Task<(bool success, string message)> DeleteAsync(int id);
}

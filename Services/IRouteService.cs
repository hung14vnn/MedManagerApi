using MedManagerApi.Models;

namespace MedManagerApi.Services;

public interface IRouteService
{
    Task<List<RouteInformation>> GetAllAsync();
    Task<RouteInformation?> GetByIdAsync(int id);
    Task<(bool success, string message, int? id)> CreateAsync(RouteInformation route);
    Task<(bool success, string message)> UpdateAsync(int id, RouteInformation route);
    Task<(bool success, string message)> DeleteAsync(int id);
}

using MedManagerApi.Models;

namespace MedManagerApi.Services;

public interface IIngredientService
{
    Task<(List<Ingredient> ingredients, int totalCount, int totalPages)> GetAllAsync(int page, int pageSize);
    Task<Ingredient?> GetByIdAsync(int id);
    Task<List<Ingredient>> SearchAsync(string query);
    Task<(bool success, string message, int? id)> CreateAsync(Ingredient ingredient);
    Task<(bool success, string message)> UpdateAsync(int id, Ingredient ingredient);
    Task<(bool success, string message)> DeleteAsync(int id);
}

using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class IngredientService : IIngredientService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<IngredientService> _logger;

    public IngredientService(MedManagerDbContext context, ILogger<IngredientService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(List<Ingredient> ingredients, int totalCount, int totalPages)> GetAllAsync(int page, int pageSize)
    {
        var totalCount = await _context.Ingredients.CountAsync();
        var ingredients = await _context.Ingredients
            .OrderBy(i => i.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return (ingredients, totalCount, totalPages);
    }

    public async Task<Ingredient?> GetByIdAsync(int id)
    {
        return await _context.Ingredients
            .Include(i => i.DrugIngredients)
                .ThenInclude(di => di.Drug)
            .Include(i => i.IngredientMechanisms)
                .ThenInclude(im => im.Mechanism)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Ingredient>> SearchAsync(string query)
    {
        return await _context.Ingredients
            .Where(i => i.Name.Contains(query) || i.Code.Contains(query))
            .OrderBy(i => i.Name)
            .Take(20)
            .ToListAsync();
    }

    public async Task<(bool success, string message, int? id)> CreateAsync(Ingredient ingredient)
    {
        try
        {
            // Check if code already exists
            if (await _context.Ingredients.AnyAsync(i => i.Code == ingredient.Code))
                return (false, "Ingredient code already exists", null);

            ingredient.CreatedAt = DateTime.UtcNow;
            ingredient.UpdatedAt = DateTime.UtcNow;

            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ingredient created: {Code} - {Name}", ingredient.Code, ingredient.Name);

            return (true, "Ingredient created successfully", ingredient.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ingredient: {Code}", ingredient.Code);
            return (false, "An error occurred while creating the ingredient", null);
        }
    }

    public async Task<(bool success, string message)> UpdateAsync(int id, Ingredient ingredient)
    {
        try
        {
            var existing = await _context.Ingredients.FindAsync(id);
            if (existing == null)
                return (false, "Ingredient not found");

            // Check if code is being changed to one that already exists
            if (existing.Code != ingredient.Code && 
                await _context.Ingredients.AnyAsync(i => i.Code == ingredient.Code))
                return (false, "Ingredient code already exists");

            existing.Code = ingredient.Code;
            existing.Name = ingredient.Name;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Ingredient updated: {Code} - {Name}", ingredient.Code, ingredient.Name);

            return (true, "Ingredient updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ingredient: {Id}", id);
            return (false, "An error occurred while updating the ingredient");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
                return (false, "Ingredient not found");

            // Check if ingredient is used in any drugs
            var isUsed = await _context.DrugIngredients.AnyAsync(di => di.IngredientId == id);
            if (isUsed)
                return (false, "Cannot delete ingredient that is used in drugs");

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ingredient deleted: {Code} - {Name}", ingredient.Code, ingredient.Name);

            return (true, "Ingredient deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ingredient: {Id}", id);
            return (false, "An error occurred while deleting the ingredient");
        }
    }
}

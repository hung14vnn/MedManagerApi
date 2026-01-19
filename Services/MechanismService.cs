using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class MechanismService : IMechanismService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<MechanismService> _logger;

    public MechanismService(MedManagerDbContext context, ILogger<MechanismService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<MechanismInformation>> GetAllAsync()
    {
        return await _context.MechanismInformations
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<MechanismInformation?> GetByIdAsync(int id)
    {
        return await _context.MechanismInformations
            .Include(m => m.IngredientMechanisms)
                .ThenInclude(im => im.Ingredient)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<(bool success, string message, int? id)> CreateAsync(MechanismInformation mechanism)
    {
        try
        {
            // Check if code already exists
            if (await _context.MechanismInformations.AnyAsync(m => m.Code == mechanism.Code))
                return (false, "Mechanism code already exists", null);

            mechanism.CreatedAt = DateTime.UtcNow;
            mechanism.UpdatedAt = DateTime.UtcNow;

            _context.MechanismInformations.Add(mechanism);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Mechanism created: {Code} - {Name}", mechanism.Code, mechanism.Name);

            return (true, "Mechanism created successfully", mechanism.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating mechanism: {Code}", mechanism.Code);
            return (false, "An error occurred while creating the mechanism", null);
        }
    }

    public async Task<(bool success, string message)> UpdateAsync(int id, MechanismInformation mechanism)
    {
        try
        {
            var existing = await _context.MechanismInformations.FindAsync(id);
            if (existing == null)
                return (false, "Mechanism not found");

            // Check if code is being changed to one that already exists
            if (existing.Code != mechanism.Code && 
                await _context.MechanismInformations.AnyAsync(m => m.Code == mechanism.Code))
                return (false, "Mechanism code already exists");

            existing.Code = mechanism.Code;
            existing.Name = mechanism.Name;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Mechanism updated: {Code} - {Name}", mechanism.Code, mechanism.Name);

            return (true, "Mechanism updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating mechanism: {Id}", id);
            return (false, "An error occurred while updating the mechanism");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var mechanism = await _context.MechanismInformations.FindAsync(id);
            if (mechanism == null)
                return (false, "Mechanism not found");

            // Check if mechanism is used in any ingredient-mechanism or interaction-mechanism relationships
            var isUsedInIngredients = await _context.IngredientMechanisms.AnyAsync(im => im.MechanismId == id);
            var isUsedInInteractions = await _context.InteractionMechanisms.AnyAsync(im => im.MechanismId == id);
            
            if (isUsedInIngredients || isUsedInInteractions)
                return (false, "Cannot delete mechanism that is used in ingredients or interactions");

            _context.MechanismInformations.Remove(mechanism);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Mechanism deleted: {Code} - {Name}", mechanism.Code, mechanism.Name);

            return (true, "Mechanism deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting mechanism: {Id}", id);
            return (false, "An error occurred while deleting the mechanism");
        }
    }
}

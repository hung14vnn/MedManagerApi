using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class DosageFormService : IDosageFormService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<DosageFormService> _logger;

    public DosageFormService(MedManagerDbContext context, ILogger<DosageFormService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DosageForm>> GetAllAsync()
    {
        return await _context.DosageForms
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<DosageForm?> GetByIdAsync(int id)
    {
        return await _context.DosageForms
            .Include(d => d.Drugs)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<(bool success, string message, int? id)> CreateAsync(DosageForm dosageForm)
    {
        try
        {
            // Check if code already exists
            if (await _context.DosageForms.AnyAsync(d => d.Code == dosageForm.Code))
                return (false, "Dosage form code already exists", null);

            dosageForm.CreatedAt = DateTime.UtcNow;
            dosageForm.UpdatedAt = DateTime.UtcNow;

            _context.DosageForms.Add(dosageForm);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dosage form created: {Code} - {Name}", dosageForm.Code, dosageForm.Name);

            return (true, "Dosage form created successfully", dosageForm.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dosage form: {Code}", dosageForm.Code);
            return (false, "An error occurred while creating the dosage form", null);
        }
    }

    public async Task<(bool success, string message)> UpdateAsync(int id, DosageForm dosageForm)
    {
        try
        {
            var existing = await _context.DosageForms.FindAsync(id);
            if (existing == null)
                return (false, "Dosage form not found");

            // Check if code is being changed to one that already exists
            if (existing.Code != dosageForm.Code && 
                await _context.DosageForms.AnyAsync(d => d.Code == dosageForm.Code))
                return (false, "Dosage form code already exists");

            existing.Code = dosageForm.Code;
            existing.Name = dosageForm.Name;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Dosage form updated: {Code} - {Name}", dosageForm.Code, dosageForm.Name);

            return (true, "Dosage form updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dosage form: {Id}", id);
            return (false, "An error occurred while updating the dosage form");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var dosageForm = await _context.DosageForms.FindAsync(id);
            if (dosageForm == null)
                return (false, "Dosage form not found");

            // Check if dosage form is used in any drugs
            var isUsed = await _context.Drugs.AnyAsync(d => d.DosageFormId == id);
            if (isUsed)
                return (false, "Cannot delete dosage form that is used in drugs");

            _context.DosageForms.Remove(dosageForm);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Dosage form deleted: {Code} - {Name}", dosageForm.Code, dosageForm.Name);

            return (true, "Dosage form deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting dosage form: {Id}", id);
            return (false, "An error occurred while deleting the dosage form");
        }
    }
}

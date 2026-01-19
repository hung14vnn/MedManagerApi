using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.DTOs;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class DrugService : IDrugService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<DrugService> _logger;

    public DrugService(MedManagerDbContext context, ILogger<DrugService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DrugSearchDto>> SearchDrugsAsync(string? searchTerm = null)
    {
        var query = _context.Drugs
            .Include(d => d.DosageForm)
            .Include(d => d.Route)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(d =>
                d.Code.ToLower().Contains(lowerSearchTerm) ||
                d.Name.ToLower().Contains(lowerSearchTerm)
            );
        }

        return await query
            .OrderBy(d => d.Name)
            .Select(d => new DrugSearchDto(
                d.Id,
                d.Code,
                d.Name,
                d.Status.ToString(),
                d.DosageForm != null ? d.DosageForm.Name : null,
                d.Route != null ? d.Route.Name : null
            ))
            .ToListAsync();
    }

    public async Task<DrugDetailDto?> GetDrugByIdAsync(int id)
    {
        var drug = await _context.Drugs
            .Include(d => d.DosageForm)
            .Include(d => d.Route)
            .Include(d => d.DrugIngredients)
                .ThenInclude(di => di.Ingredient)
            .Include(d => d.References)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (drug == null) return null;

        return MapToDrugDetailDto(drug);
    }

    public async Task<DrugDetailDto> CreateDrugAsync(CreateDrugDto dto)
    {
        try
        {
            // Parse status
            if (!Enum.TryParse<DrugStatus>(dto.Status, out var status))
            {
                throw new ArgumentException($"Invalid status value: {dto.Status}");
            }

            var drug = new Drug
            {
                Code = dto.Code,
                Name = dto.Name,
                Status = status,
                DosageFormId = dto.DosageFormId,
                RouteId = dto.RouteId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Drugs.Add(drug);
            await _context.SaveChangesAsync();

            // Add ingredients
            if (dto.Ingredients != null && dto.Ingredients.Any())
            {
                foreach (var ingredientDto in dto.Ingredients)
                {
                    var drugIngredient = new DrugIngredient
                    {
                        DrugId = drug.Id,
                        IngredientId = ingredientDto.IngredientId,
                        Strength = ingredientDto.Strength,
                        Unit = ingredientDto.Unit,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.DrugIngredients.Add(drugIngredient);
                }
                await _context.SaveChangesAsync();
            }

            // Reload with all relationships
            var createdDrug = await _context.Drugs
                .Include(d => d.DosageForm)
                .Include(d => d.Route)
                .Include(d => d.DrugIngredients)
                    .ThenInclude(di => di.Ingredient)
                .Include(d => d.References)
                .FirstAsync(d => d.Id == drug.Id);

            _logger.LogInformation("Drug created: {Code} - {Name}", drug.Code, drug.Name);

            return MapToDrugDetailDto(createdDrug);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating drug: {Code}", dto.Code);
            throw;
        }
    }

    public async Task<DrugDetailDto?> UpdateDrugAsync(int id, UpdateDrugDto dto)
    {
        try
        {
            var drug = await _context.Drugs
                .Include(d => d.DrugIngredients)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (drug == null) return null;

            // Parse status
            if (!Enum.TryParse<DrugStatus>(dto.Status, out var status))
            {
                throw new ArgumentException($"Invalid status value: {dto.Status}");
            }

            drug.Code = dto.Code;
            drug.Name = dto.Name;
            drug.Status = status;
            drug.DosageFormId = dto.DosageFormId;
            drug.RouteId = dto.RouteId;
            drug.UpdatedAt = DateTime.UtcNow;

            // Update ingredients - remove old ones and add new ones
            _context.DrugIngredients.RemoveRange(drug.DrugIngredients);

            if (dto.Ingredients != null && dto.Ingredients.Any())
            {
                foreach (var ingredientDto in dto.Ingredients)
                {
                    var drugIngredient = new DrugIngredient
                    {
                        DrugId = drug.Id,
                        IngredientId = ingredientDto.IngredientId,
                        Strength = ingredientDto.Strength,
                        Unit = ingredientDto.Unit,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.DrugIngredients.Add(drugIngredient);
                }
            }

            await _context.SaveChangesAsync();

            // Reload with all relationships
            var updatedDrug = await _context.Drugs
                .Include(d => d.DosageForm)
                .Include(d => d.Route)
                .Include(d => d.DrugIngredients)
                    .ThenInclude(di => di.Ingredient)
                .Include(d => d.References)
                .FirstAsync(d => d.Id == id);

            _logger.LogInformation("Drug updated: {Code} - {Name}", drug.Code, drug.Name);

            return MapToDrugDetailDto(updatedDrug);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating drug: {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDrugAsync(int id)
    {
        var drug = await _context.Drugs.FindAsync(id);
        if (drug == null) return false;

        _context.Drugs.Remove(drug);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Drug deleted: {Id}", id);

        return true;
    }

    public async Task<ReferenceDto> AddReferenceAsync(int drugId, CreateReferenceDto dto)
    {
        var reference = new DrugReference
        {
            DrugId = drugId,
            Title = dto.Title,
            Authors = dto.Authors,
            Source = dto.Source,
            Url = dto.Url,
            PublicationDate = dto.PublicationDate,
            Doi = dto.Doi
        };

        _context.DrugReferences.Add(reference);
        await _context.SaveChangesAsync();

        return new ReferenceDto(
            reference.Id,
            reference.Title,
            reference.Authors,
            reference.Source,
            reference.Url,
            reference.PublicationDate,
            reference.Doi
        );
    }

    private static DrugDetailDto MapToDrugDetailDto(Drug drug)
    {
        return new DrugDetailDto(
            drug.Id,
            drug.Code,
            drug.Name,
            drug.Status.ToString(),
            drug.DosageForm != null ? new DosageFormDto(
                drug.DosageForm.Id,
                drug.DosageForm.Code,
                drug.DosageForm.Name
            ) : null,
            drug.Route != null ? new RouteDto(
                drug.Route.Id,
                drug.Route.Code,
                drug.Route.Name
            ) : null,
            drug.DrugIngredients.Select(di => new DrugIngredientDto(
                di.Id,
                new IngredientDto(
                    di.Ingredient.Id,
                    di.Ingredient.Code,
                    di.Ingredient.Name
                ),
                di.Strength,
                di.Unit
            )).ToList(),
            drug.References.Select(r => new ReferenceDto(
                r.Id,
                r.Title,
                r.Authors,
                r.Source,
                r.Url,
                r.PublicationDate,
                r.Doi
            )).ToList(),
            drug.CreatedAt,
            drug.UpdatedAt
        );
    }
}

using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.DTOs;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class DrugService : IDrugService
{
    private readonly MedManagerDbContext _context;

    public DrugService(MedManagerDbContext context)
    {
        _context = context;
    }

    public async Task<List<DrugSearchDto>> SearchDrugsAsync(string? searchTerm = null)
    {
        var query = _context.Drugs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(d =>
                d.ActiveIngredient.ToLower().Contains(lowerSearchTerm) ||
                d.BrandName.ToLower().Contains(lowerSearchTerm) ||
                (d.PharmacologicalGroup != null && d.PharmacologicalGroup.ToLower().Contains(lowerSearchTerm))
            );
        }

        // Apply ordering on the entity property before projecting to DTO so EF Core can translate the query.
        return await query
            .OrderBy(d => d.ActiveIngredient)
            .Select(d => new DrugSearchDto(
                d.Id,
                d.ActiveIngredient,
                d.BrandName,
                d.PharmacologicalGroup
            ))
            .ToListAsync();
    }

    public async Task<DrugDetailDto?> GetDrugByIdAsync(int id)
    {
        var drug = await _context.Drugs
            .Include(d => d.References)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (drug == null) return null;

        return MapToDrugDetailDto(drug);
    }

    public async Task<DrugDetailDto> CreateDrugAsync(CreateDrugDto dto)
    {
        var drug = new Drug
        {
            ActiveIngredient = dto.ActiveIngredient,
            BrandName = dto.BrandName,
            PharmacologicalGroup = dto.PharmacologicalGroup,
            Indications = dto.Indications,
            Contraindications = dto.Contraindications,
            DosageAdults = dto.DosageAdults,
            DosageChildren = dto.DosageChildren,
            DosageHepaticImpairment = dto.DosageHepaticImpairment,
            DosageRenalImpairment = dto.DosageRenalImpairment,
            AdverseEffects = dto.AdverseEffects,
            PregnancyPrecautions = dto.PregnancyPrecautions,
            BreastfeedingPrecautions = dto.BreastfeedingPrecautions,
            OtherPrecautions = dto.OtherPrecautions
        };

        _context.Drugs.Add(drug);
        await _context.SaveChangesAsync();

        return MapToDrugDetailDto(drug);
    }

    public async Task<DrugDetailDto?> UpdateDrugAsync(int id, CreateDrugDto dto)
    {
        var drug = await _context.Drugs
            .Include(d => d.References)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (drug == null) return null;

        drug.ActiveIngredient = dto.ActiveIngredient;
        drug.BrandName = dto.BrandName;
        drug.PharmacologicalGroup = dto.PharmacologicalGroup;
        drug.Indications = dto.Indications;
        drug.Contraindications = dto.Contraindications;
        drug.DosageAdults = dto.DosageAdults;
        drug.DosageChildren = dto.DosageChildren;
        drug.DosageHepaticImpairment = dto.DosageHepaticImpairment;
        drug.DosageRenalImpairment = dto.DosageRenalImpairment;
        drug.AdverseEffects = dto.AdverseEffects;
        drug.PregnancyPrecautions = dto.PregnancyPrecautions;
        drug.BreastfeedingPrecautions = dto.BreastfeedingPrecautions;
        drug.OtherPrecautions = dto.OtherPrecautions;
        drug.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDrugDetailDto(drug);
    }

    public async Task<bool> DeleteDrugAsync(int id)
    {
        var drug = await _context.Drugs.FindAsync(id);
        if (drug == null) return false;

        _context.Drugs.Remove(drug);
        await _context.SaveChangesAsync();
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
            drug.ActiveIngredient,
            drug.BrandName,
            drug.PharmacologicalGroup,
            drug.Indications,
            drug.Contraindications,
            drug.DosageAdults,
            drug.DosageChildren,
            drug.DosageHepaticImpairment,
            drug.DosageRenalImpairment,
            drug.AdverseEffects,
            drug.PregnancyPrecautions,
            drug.BreastfeedingPrecautions,
            drug.OtherPrecautions,
            drug.References.Select(r => new ReferenceDto(
                r.Id,
                r.Title,
                r.Authors,
                r.Source,
                r.Url,
                r.PublicationDate,
                r.Doi
            )).ToList()
        );
    }
}

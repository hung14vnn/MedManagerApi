using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.DTOs;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class DiseaseService : IDiseaseService
{
    private readonly MedManagerDbContext _context;

    public DiseaseService(MedManagerDbContext context)
    {
        _context = context;
    }

    public async Task<List<DiseaseDto>> GetAllDiseasesAsync()
    {
        // Order in the database before projecting to DiseaseDto so EF Core can translate the query.
        return await _context.Diseases
            .OrderBy(d => d.Name)
            .Select(d => new DiseaseDto(d.Id, d.Name, d.IcdCode, d.Description))
            .ToListAsync();
    }

    public async Task<DiseaseDto?> GetDiseaseByIdAsync(int id)
    {
        var disease = await _context.Diseases.FindAsync(id);
        if (disease == null) return null;

        return new DiseaseDto(disease.Id, disease.Name, disease.IcdCode, disease.Description);
    }

    public async Task<DiseaseTreatmentDto?> GetTreatmentProtocolAsync(int diseaseId)
    {
        var disease = await _context.Diseases.FindAsync(diseaseId);
        if (disease == null) return null;

        var protocols = await _context.DiseaseProtocols
            .Include(dp => dp.Drug)
            .Where(dp => dp.DiseaseId == diseaseId)
            .OrderBy(dp => dp.PreferenceOrder)
            .ToListAsync();

        var preferredDrugs = protocols
            .Where(dp => dp.IsPreferred)
            .Select(dp => new TreatmentOptionDto(
                new DrugSearchDto(
                    dp.Drug.Id,
                    dp.Drug.ActiveIngredient,
                    dp.Drug.BrandName,
                    dp.Drug.PharmacologicalGroup
                ),
                dp.DosageRecommendation,
                dp.SpecialConditions,
                dp.Notes
            ))
            .ToList();

        var alternativeDrugs = protocols
            .Where(dp => !dp.IsPreferred)
            .Select(dp => new TreatmentOptionDto(
                new DrugSearchDto(
                    dp.Drug.Id,
                    dp.Drug.ActiveIngredient,
                    dp.Drug.BrandName,
                    dp.Drug.PharmacologicalGroup
                ),
                dp.DosageRecommendation,
                dp.SpecialConditions,
                dp.Notes
            ))
            .ToList();

        return new DiseaseTreatmentDto(
            new DiseaseDto(disease.Id, disease.Name, disease.IcdCode, disease.Description),
            preferredDrugs,
            alternativeDrugs
        );
    }

    public async Task<DiseaseDto> CreateDiseaseAsync(CreateDiseaseDto dto)
    {
        var disease = new Disease
        {
            Name = dto.Name,
            IcdCode = dto.IcdCode,
            Description = dto.Description
        };

        _context.Diseases.Add(disease);
        await _context.SaveChangesAsync();

        return new DiseaseDto(disease.Id, disease.Name, disease.IcdCode, disease.Description);
    }

    public async Task<DiseaseProtocolDto> AddTreatmentProtocolAsync(CreateProtocolDto dto)
    {
        var protocol = new DiseaseProtocol
        {
            DiseaseId = dto.DiseaseId,
            DrugId = dto.DrugId,
            IsPreferred = dto.IsPreferred,
            PreferenceOrder = dto.PreferenceOrder,
            DosageRecommendation = dto.DosageRecommendation,
            SpecialConditions = dto.SpecialConditions,
            Notes = dto.Notes
        };

        _context.DiseaseProtocols.Add(protocol);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var createdProtocol = await _context.DiseaseProtocols
            .Include(dp => dp.Disease)
            .Include(dp => dp.Drug)
            .FirstAsync(dp => dp.Id == protocol.Id);

        return new DiseaseProtocolDto(
            createdProtocol.Id,
            new DiseaseDto(
                createdProtocol.Disease.Id,
                createdProtocol.Disease.Name,
                createdProtocol.Disease.IcdCode,
                createdProtocol.Disease.Description
            ),
            new DrugSearchDto(
                createdProtocol.Drug.Id,
                createdProtocol.Drug.ActiveIngredient,
                createdProtocol.Drug.BrandName,
                createdProtocol.Drug.PharmacologicalGroup
            ),
            createdProtocol.IsPreferred,
            createdProtocol.PreferenceOrder,
            createdProtocol.DosageRecommendation,
            createdProtocol.SpecialConditions,
            createdProtocol.Notes
        );
    }
}

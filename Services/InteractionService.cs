using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.DTOs;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class InteractionService : IInteractionService
{
    private readonly MedManagerDbContext _context;

    public InteractionService(MedManagerDbContext context)
    {
        _context = context;
    }

    public async Task<InteractionCheckResponse> CheckInteractionsAsync(List<int> drugIds)
    {
        if (drugIds == null || drugIds.Count < 2)
        {
            return new InteractionCheckResponse(new List<InteractionDetailDto>(), "None");
        }

        var interactions = new List<InteractionDetailDto>();

        // Check for interactions between all pairs of drugs
        for (int i = 0; i < drugIds.Count; i++)
        {
            for (int j = i + 1; j < drugIds.Count; j++)
            {
                var interaction = await _context.DrugInteractions
                    .Include(di => di.Drug1)
                        .ThenInclude(d => d.DosageForm)
                    .Include(di => di.Drug1)
                        .ThenInclude(d => d.Route)
                    .Include(di => di.Drug2)
                        .ThenInclude(d => d.DosageForm)
                    .Include(di => di.Drug2)
                        .ThenInclude(d => d.Route)
                    .Include(di => di.References)
                    .Where(di =>
                        (di.Drug1Id == drugIds[i] && di.Drug2Id == drugIds[j]) ||
                        (di.Drug1Id == drugIds[j] && di.Drug2Id == drugIds[i])
                    )
                    .FirstOrDefaultAsync();

                if (interaction != null)
                {
                    interactions.Add(MapToInteractionDetailDto(interaction));
                }
            }
        }

        // Determine overall severity
        var overallSeverity = "None";
        if (interactions.Any(i => i.Severity == "Severe"))
            overallSeverity = "Severe";
        else if (interactions.Any(i => i.Severity == "Moderate"))
            overallSeverity = "Moderate";
        else if (interactions.Any(i => i.Severity == "Mild"))
            overallSeverity = "Mild";

        return new InteractionCheckResponse(interactions, overallSeverity);
    }

    public async Task<InteractionDetailDto?> GetInteractionByIdAsync(int id)
    {
        var interaction = await _context.DrugInteractions
            .Include(di => di.Drug1)
                .ThenInclude(d => d.DosageForm)
            .Include(di => di.Drug1)
                .ThenInclude(d => d.Route)
            .Include(di => di.Drug2)
                .ThenInclude(d => d.DosageForm)
            .Include(di => di.Drug2)
                .ThenInclude(d => d.Route)
            .Include(di => di.References)
            .FirstOrDefaultAsync(di => di.Id == id);

        if (interaction == null) return null;

        return MapToInteractionDetailDto(interaction);
    }

    public async Task<InteractionDetailDto> CreateInteractionAsync(CreateInteractionDto dto)
    {
        // Parse severity
        if (!Enum.TryParse<InteractionSeverity>(dto.Severity, true, out var severity))
        {
            throw new ArgumentException($"Invalid severity: {dto.Severity}");
        }

        var interaction = new DrugInteraction
        {
            Drug1Id = dto.Drug1Id,
            Drug2Id = dto.Drug2Id,
            Severity = severity,
            Mechanism = dto.Mechanism,
            ClinicalEffects = dto.ClinicalEffects,
            ManagementRecommendations = dto.ManagementRecommendations
        };

        _context.DrugInteractions.Add(interaction);
        await _context.SaveChangesAsync();

        // Reload with navigation properties
        var createdInteraction = await _context.DrugInteractions
            .Include(di => di.Drug1)
                .ThenInclude(d => d.DosageForm)
            .Include(di => di.Drug1)
                .ThenInclude(d => d.Route)
            .Include(di => di.Drug2)
                .ThenInclude(d => d.DosageForm)
            .Include(di => di.Drug2)
                .ThenInclude(d => d.Route)
            .Include(di => di.References)
            .FirstAsync(di => di.Id == interaction.Id);

        return MapToInteractionDetailDto(createdInteraction);
    }

    public async Task<bool> DeleteInteractionAsync(int id)
    {
        var interaction = await _context.DrugInteractions.FindAsync(id);
        if (interaction == null) return false;

        _context.DrugInteractions.Remove(interaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ReferenceDto> AddInteractionReferenceAsync(int interactionId, CreateReferenceDto dto)
    {
        var reference = new InteractionReference
        {
            InteractionId = interactionId,
            Title = dto.Title,
            Authors = dto.Authors,
            Source = dto.Source,
            Url = dto.Url,
            PublicationDate = dto.PublicationDate,
            Doi = dto.Doi
        };

        _context.InteractionReferences.Add(reference);
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

        private static InteractionDetailDto MapToInteractionDetailDto(DrugInteraction interaction)
        {
            return new InteractionDetailDto(
                interaction.Id,
                new DrugSearchDto(
                    interaction.Drug1.Id,
                    interaction.Drug1.Code,
                    interaction.Drug1.Name,
                    interaction.Drug1.Status.ToString(),
                    interaction.Drug1.DosageForm != null ? interaction.Drug1.DosageForm.Name : null,
                    interaction.Drug1.Route != null ? interaction.Drug1.Route.Name : null
                ),
                new DrugSearchDto(
                    interaction.Drug2.Id,
                    interaction.Drug2.Code,
                    interaction.Drug2.Name,
                    interaction.Drug2.Status.ToString(),
                    interaction.Drug2.DosageForm != null ? interaction.Drug2.DosageForm.Name : null,
                    interaction.Drug2.Route != null ? interaction.Drug2.Route.Name : null
                ),
                            interaction.Severity.ToString(),
                            interaction.Mechanism,
                            interaction.ClinicalEffects,
                            interaction.ManagementRecommendations,
                            interaction.References.Select(r => new ReferenceDto(
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

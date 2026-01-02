using MedManagerApi.DTOs;

namespace MedManagerApi.Services;

public interface IInteractionService
{
    Task<InteractionCheckResponse> CheckInteractionsAsync(List<int> drugIds);
    Task<InteractionDetailDto?> GetInteractionByIdAsync(int id);
    Task<InteractionDetailDto> CreateInteractionAsync(CreateInteractionDto dto);
    Task<bool> DeleteInteractionAsync(int id);
    Task<ReferenceDto> AddInteractionReferenceAsync(int interactionId, CreateReferenceDto dto);
}

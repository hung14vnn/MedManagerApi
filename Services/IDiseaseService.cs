using MedManagerApi.DTOs;

namespace MedManagerApi.Services;

public interface IDiseaseService
{
    Task<List<DiseaseDto>> GetAllDiseasesAsync();
    Task<DiseaseDto?> GetDiseaseByIdAsync(int id);
    Task<DiseaseTreatmentDto?> GetTreatmentProtocolAsync(int diseaseId);
    Task<DiseaseDto> CreateDiseaseAsync(CreateDiseaseDto dto);
    Task<DiseaseProtocolDto> AddTreatmentProtocolAsync(CreateProtocolDto dto);
}

using MedManagerApi.Models;

namespace MedManagerApi.Services;

public interface ITokenService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
}

using MedManagerApi.DTOs;

namespace MedManagerApi.Services;

public interface IUserManagementService
{
    Task<(IEnumerable<UserDto> users, int totalUsers, int totalPages)> GetUsersAsync(int page, int pageSize);
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<(bool success, string message, string? userId)> CreateUserAsync(CreateUserDto dto);
    Task<(bool success, string message)> UpdateUserAsync(string id, UpdateUserDto dto);
    Task<(bool success, string message)> DeactivateUserAsync(string id);
    Task<(bool success, string message)> ActivateUserAsync(string id);
    Task<(bool success, string message)> AssignRoleAsync(string id, AssignRoleDto dto);
    Task<(bool success, string message)> RemoveRoleAsync(string id, AssignRoleDto dto);
    Task<(bool success, string message)> DeleteUserAsync(string id);
}
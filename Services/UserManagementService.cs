using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedManagerApi.DTOs;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserManagementService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<(IEnumerable<UserDto> users, int totalUsers, int totalPages)> GetUsersAsync(int page, int pageSize)
    {
        var users = _userManager.Users
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = roles.ToList()
            });
        }

        var totalUsers = await _userManager.Users.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

        return (userDtos, totalUsers, totalPages);
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = roles.ToList()
        };

        return userDto;
    }

    public async Task<(bool success, string message, string? userId)> CreateUserAsync(CreateUserDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            return (false, "User with this email already exists", null);

        if (!AppRoles.GetAllRoles().Contains(dto.Role))
            return (false, $"Invalid role. Valid roles are: {string.Join(", ", AppRoles.GetAllRoles())}", null);

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            EmailConfirmed = true // Super admin created users are auto-confirmed
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)), null);

        await _userManager.AddToRoleAsync(user, dto.Role);

        _logger.LogInformation("User {Email} created by super admin", user.Email);

        return (true, "User created successfully", user.Id);
    }

    public async Task<(bool success, string message)> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found");

        if (dto.FirstName != null)
            user.FirstName = dto.FirstName;
        if (dto.LastName != null)
            user.LastName = dto.LastName;
        if (dto.IsActive.HasValue)
            user.IsActive = dto.IsActive.Value;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("User {Email} updated by super admin", user.Email);

        return (true, "User updated successfully");
    }

    public async Task<(bool success, string message)> DeactivateUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found");

        user.IsActive = false;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("User {Email} deactivated by super admin", user.Email);

        return (true, "User deactivated successfully");
    }

    public async Task<(bool success, string message)> ActivateUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found");

        user.IsActive = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("User {Email} activated by super admin", user.Email);

        return (true, "User activated successfully");
    }

    public async Task<(bool success, string message)> AssignRoleAsync(string id, AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found");

        if (!AppRoles.GetAllRoles().Contains(dto.Role))
            return (false, $"Invalid role. Valid roles are: {string.Join(", ", AppRoles.GetAllRoles())}");

        if (await _userManager.IsInRoleAsync(user, dto.Role))
            return (false, $"User already has the {dto.Role} role");

        var result = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("Role {Role} assigned to {Email} by super admin", dto.Role, user.Email);

        return (true, $"Role {dto.Role} assigned successfully");
    }

    public async Task<(bool success, string message)> RemoveRoleAsync(string id, AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found");

        if (!await _userManager.IsInRoleAsync(user, dto.Role))
            return (false, $"User does not have the {dto.Role} role");

        var result = await _userManager.RemoveFromRoleAsync(user, dto.Role);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("Role {Role} removed from {Email} by super admin", dto.Role, user.Email);

        return (true, $"Role {dto.Role} removed successfully");
    }

    public async Task<(bool success, string message)> DeleteUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return (false, "User not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        _logger.LogInformation("User {Email} deleted by super admin", user.Email);

        return (true, "User deleted successfully");
    }
}
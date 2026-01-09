using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManagerApi.DTOs;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/superadmin")]
[Authorize(Roles = AppRoles.SuperAdmin)]
public class SuperAdminController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public SuperAdminController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (users, totalUsers, totalPages) = await _userManagementService.GetUsersAsync(page, pageSize);

        return Ok(new
        {
            users,
            pagination = new
            {
                currentPage = page,
                pageSize,
                totalUsers,
                totalPages
            }
        });
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManagementService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message, userId) = await _userManagementService.CreateUserAsync(dto);
        if (!success)
            return BadRequest(new { message });

        return CreatedAtAction(nameof(GetUser), new { id = userId }, new { message, userId });
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message) = await _userManagementService.UpdateUserAsync(id, dto);
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    [HttpPost("users/{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(string id)
    {
        var (success, message) = await _userManagementService.DeactivateUserAsync(id);
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    [HttpPost("users/{id}/activate")]
    public async Task<IActionResult> ActivateUser(string id)
    {
        var (success, message) = await _userManagementService.ActivateUserAsync(id);
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    [HttpPost("users/{id}/assign-role")]
    public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleDto dto)
    {
        var (success, message) = await _userManagementService.AssignRoleAsync(id, dto);
        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpPost("users/{id}/remove-role")]
    public async Task<IActionResult> RemoveRole(string id, [FromBody] AssignRoleDto dto)
    {
        var (success, message) = await _userManagementService.RemoveRoleAsync(id, dto);
        if (!success)
            return BadRequest(new { message });

        return Ok(new { message });
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var (success, message) = await _userManagementService.DeleteUserAsync(id);
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }
}
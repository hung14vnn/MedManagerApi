using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MechanismsController : ControllerBase
{
    private readonly IMechanismService _mechanismService;

    public MechanismsController(IMechanismService mechanismService)
    {
        _mechanismService = mechanismService;
    }

    /// <summary>
    /// Get all mechanisms
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var mechanisms = await _mechanismService.GetAllAsync();
        return Ok(mechanisms);
    }

    /// <summary>
    /// Get mechanism by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var mechanism = await _mechanismService.GetByIdAsync(id);

        if (mechanism == null)
            return NotFound(new { message = "Mechanism not found" });

        return Ok(mechanism);
    }

    /// <summary>
    /// Create new mechanism (SuperAdmin and Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] MechanismInformation mechanism)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message, id) = await _mechanismService.CreateAsync(mechanism);
        
        if (!success)
            return BadRequest(new { message });

        return CreatedAtAction(nameof(GetById), new { id }, new { message, id });
    }

    /// <summary>
    /// Update mechanism (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Update(int id, [FromBody] MechanismInformation mechanism)
    {
        if (id != mechanism.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message) = await _mechanismService.UpdateAsync(id, mechanism);
        
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    /// <summary>
    /// Delete mechanism (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _mechanismService.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }
}

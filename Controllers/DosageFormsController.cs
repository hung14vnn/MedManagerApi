using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DosageFormsController : ControllerBase
{
    private readonly IDosageFormService _dosageFormService;

    public DosageFormsController(IDosageFormService dosageFormService)
    {
        _dosageFormService = dosageFormService;
    }

    /// <summary>
    /// Get all dosage forms
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var dosageForms = await _dosageFormService.GetAllAsync();
        return Ok(dosageForms);
    }

    /// <summary>
    /// Get dosage form by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dosageForm = await _dosageFormService.GetByIdAsync(id);

        if (dosageForm == null)
            return NotFound(new { message = "Dosage form not found" });

        return Ok(dosageForm);
    }

    /// <summary>
    /// Create new dosage form (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] DosageForm dosageForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message, id) = await _dosageFormService.CreateAsync(dosageForm);

        if (!success)
            return BadRequest(new { message });

        return CreatedAtAction(nameof(GetById), new { id }, new { message, id });
    }

    /// <summary>
    /// Update dosage form (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Update(int id, [FromBody] DosageForm dosageForm)
    {
        if (id != dosageForm.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message) = await _dosageFormService.UpdateAsync(id, dosageForm);

        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    /// <summary>
    /// Delete dosage form (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _dosageFormService.DeleteAsync(id);

        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedManagerApi.DTOs;
using MedManagerApi.Services;
using MedManagerApi.Models;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrugsController : ControllerBase
{
    private readonly IDrugService _drugService;
    private readonly ISearchLogService _searchLogService;

    public DrugsController(IDrugService drugService, ISearchLogService searchLogService)
    {
        _drugService = drugService;
        _searchLogService = searchLogService;
    }

    // Public endpoint - anyone can search drugs
    [HttpGet]
    public async Task<ActionResult<List<DrugSearchDto>>> SearchDrugs([FromQuery] string? search = null)
    {
        var drugs = await _drugService.SearchDrugsAsync(search);

        // Log search
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        await _searchLogService.LogSearchAsync(
            search ?? "",
            SearchEntityType.Drug,
            drugs.Count,
            userId,
            ipAddress,
            userAgent
        );

        return Ok(drugs);
    }

    // Public endpoint - anyone can view drug details
    [HttpGet("{id}")]
    public async Task<ActionResult<DrugDetailDto>> GetDrug(int id)
    {
        var drug = await _drugService.GetDrugByIdAsync(id);
        if (drug == null)
            return NotFound($"Drug with ID {id} not found");

        return Ok(drug);
    }

    // SuperAdmin, Admin and Pharmacist only - create a new drug
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin},Pharmacist")]
    public async Task<ActionResult<DrugDetailDto>> CreateDrug(CreateDrugDto dto)
    {
        var drug = await _drugService.CreateDrugAsync(dto);
        return CreatedAtAction(nameof(GetDrug), new { id = drug.Id }, drug);
    }

    // SuperAdmin, Admin and Pharmacist only - update an existing drug
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin},Pharmacist")]
    public async Task<ActionResult<DrugDetailDto>> UpdateDrug(int id, UpdateDrugDto dto)
    {
        var drug = await _drugService.UpdateDrugAsync(id, dto);
        if (drug == null)
            return NotFound($"Drug with ID {id} not found");

        return Ok(drug);
    }

    // SuperAdmin and Admin only - delete a drug
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> DeleteDrug(int id)
    {
        var result = await _drugService.DeleteDrugAsync(id);
        if (!result)
            return NotFound($"Drug with ID {id} not found");

        return NoContent();
    }

    // SuperAdmin, Admin and Pharmacist only - add a reference to a drug
    [HttpPost("{id}/references")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin},Pharmacist")]
    public async Task<ActionResult<ReferenceDto>> AddReference(int id, CreateReferenceDto dto)
    {
        try
        {
            var reference = await _drugService.AddReferenceAsync(id, dto);
            return Ok(reference);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

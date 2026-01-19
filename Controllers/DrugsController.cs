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

    public DrugsController(IDrugService drugService)
    {
        _drugService = drugService;
    }

    // Public endpoint - anyone can search drugs
    [HttpGet]
    public async Task<ActionResult<List<DrugSearchDto>>> SearchDrugs([FromQuery] string? search = null)
    {
        var drugs = await _drugService.SearchDrugsAsync(search);
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

    // Admin and Pharmacist only - create a new drug
    [HttpPost]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<ActionResult<DrugDetailDto>> CreateDrug(CreateDrugDto dto)
    {
        var drug = await _drugService.CreateDrugAsync(dto);
        return CreatedAtAction(nameof(GetDrug), new { id = drug.Id }, drug);
    }

    // Admin and Pharmacist only - update an existing drug
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Pharmacist")]
    public async Task<ActionResult<DrugDetailDto>> UpdateDrug(int id, UpdateDrugDto dto)
    {
        var drug = await _drugService.UpdateDrugAsync(id, dto);
        if (drug == null)
            return NotFound($"Drug with ID {id} not found");

        return Ok(drug);
    }

    // Admin only - delete a drug
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDrug(int id)
    {
        var result = await _drugService.DeleteDrugAsync(id);
        if (!result)
            return NotFound($"Drug with ID {id} not found");

        return NoContent();
    }

    // Admin and Pharmacist only - add a reference to a drug
    [HttpPost("{id}/references")]
    [Authorize(Roles = "Admin,Pharmacist")]
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

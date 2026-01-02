using Microsoft.AspNetCore.Mvc;
using MedManagerApi.DTOs;
using MedManagerApi.Services;

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

    [HttpGet]
    public async Task<ActionResult<List<DrugSearchDto>>> SearchDrugs([FromQuery] string? search = null)
    {
        var drugs = await _drugService.SearchDrugsAsync(search);
        return Ok(drugs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DrugDetailDto>> GetDrug(int id)
    {
        var drug = await _drugService.GetDrugByIdAsync(id);
        if (drug == null)
            return NotFound($"Drug with ID {id} not found");

        return Ok(drug);
    }

    [HttpPost]
    public async Task<ActionResult<DrugDetailDto>> CreateDrug(CreateDrugDto dto)
    {
        var drug = await _drugService.CreateDrugAsync(dto);
        return CreatedAtAction(nameof(GetDrug), new { id = drug.Id }, drug);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DrugDetailDto>> UpdateDrug(int id, CreateDrugDto dto)
    {
        var drug = await _drugService.UpdateDrugAsync(id, dto);
        if (drug == null)
            return NotFound($"Drug with ID {id} not found");

        return Ok(drug);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDrug(int id)
    {
        var result = await _drugService.DeleteDrugAsync(id);
        if (!result)
            return NotFound($"Drug with ID {id} not found");

        return NoContent();
    }

    [HttpPost("{id}/references")]
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

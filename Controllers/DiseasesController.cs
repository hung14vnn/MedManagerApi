using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedManagerApi.DTOs;
using MedManagerApi.Services;
using MedManagerApi.Models;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiseasesController : ControllerBase
{
    private readonly IDiseaseService _diseaseService;
    private readonly ISearchLogService _searchLogService;

    public DiseasesController(IDiseaseService diseaseService, ISearchLogService searchLogService)
    {
        _diseaseService = diseaseService;
        _searchLogService = searchLogService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DiseaseDto>>> GetAllDiseases()
    {
        var diseases = await _diseaseService.GetAllDiseasesAsync();
        return Ok(diseases);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiseaseDto>> GetDisease(int id)
    {
        var disease = await _diseaseService.GetDiseaseByIdAsync(id);
        if (disease == null)
            return NotFound($"Disease with ID {id} not found");

        return Ok(disease);
    }

    [HttpGet("{id}/treatment")]
    public async Task<ActionResult<DiseaseTreatmentDto>> GetTreatmentProtocol(int id)
    {
        var treatment = await _diseaseService.GetTreatmentProtocolAsync(id);
        if (treatment == null)
            return NotFound($"Disease with ID {id} not found");

        // Log search
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        await _searchLogService.LogSearchAsync(
            $"Disease treatment: {treatment.Disease.Name}",
            SearchEntityType.Disease,
            treatment.PreferredDrugs.Count + treatment.AlternativeDrugs.Count,
            userId,
            ipAddress,
            userAgent
        );

        return Ok(treatment);
    }

    [HttpPost]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<ActionResult<DiseaseDto>> CreateDisease(CreateDiseaseDto dto)
    {
        var disease = await _diseaseService.CreateDiseaseAsync(dto);
        return CreatedAtAction(nameof(GetDisease), new { id = disease.Id }, disease);
    }

    [HttpPost("protocols")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<ActionResult<DiseaseProtocolDto>> AddTreatmentProtocol(CreateProtocolDto dto)
    {
        try
        {
            var protocol = await _diseaseService.AddTreatmentProtocolAsync(dto);
            return Ok(protocol);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

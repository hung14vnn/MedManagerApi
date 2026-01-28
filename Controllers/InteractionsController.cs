using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MedManagerApi.DTOs;
using MedManagerApi.Services;
using MedManagerApi.Models;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InteractionsController : ControllerBase
{
    private readonly IInteractionService _interactionService;
    private readonly ISearchLogService _searchLogService;

    public InteractionsController(IInteractionService interactionService, ISearchLogService searchLogService)
    {
        _interactionService = interactionService;
        _searchLogService = searchLogService;
    }

    [HttpPost("check")]
    public async Task<ActionResult<InteractionCheckResponse>> CheckInteractions(InteractionCheckRequest request)
    {
        if (request.DrugIds == null || request.DrugIds.Count < 2)
        {
            return BadRequest("Please provide at least 2 drug IDs to check for interactions");
        }

        var result = await _interactionService.CheckInteractionsAsync(request.DrugIds);

        // Log search
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        await _searchLogService.LogSearchAsync(
            $"Drug IDs: {string.Join(", ", request.DrugIds)}",
            SearchEntityType.Interaction,
            result.Interactions.Count,
            userId,
            ipAddress,
            userAgent
        );

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InteractionDetailDto>> GetInteraction(int id)
    {
        var interaction = await _interactionService.GetInteractionByIdAsync(id);
        if (interaction == null)
            return NotFound($"Interaction with ID {id} not found");

        return Ok(interaction);
    }

    [HttpPost]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<ActionResult<InteractionDetailDto>> CreateInteraction(CreateInteractionDto dto)
    {
        try
        {
            var interaction = await _interactionService.CreateInteractionAsync(dto);
            return CreatedAtAction(nameof(GetInteraction), new { id = interaction.Id }, interaction);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> DeleteInteraction(int id)
    {
        var result = await _interactionService.DeleteInteractionAsync(id);
        if (!result)
            return NotFound($"Interaction with ID {id} not found");

        return NoContent();
    }

    [HttpPost("{id}/references")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<ActionResult<ReferenceDto>> AddReference(int id, CreateReferenceDto dto)
    {
        try
        {
            var reference = await _interactionService.AddInteractionReferenceAsync(id, dto);
            return Ok(reference);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

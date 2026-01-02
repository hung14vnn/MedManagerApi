using Microsoft.AspNetCore.Mvc;
using MedManagerApi.DTOs;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InteractionsController : ControllerBase
{
    private readonly IInteractionService _interactionService;

    public InteractionsController(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    [HttpPost("check")]
    public async Task<ActionResult<InteractionCheckResponse>> CheckInteractions(InteractionCheckRequest request)
    {
        if (request.DrugIds == null || request.DrugIds.Count < 2)
        {
            return BadRequest("Please provide at least 2 drug IDs to check for interactions");
        }

        var result = await _interactionService.CheckInteractionsAsync(request.DrugIds);
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
    public async Task<IActionResult> DeleteInteraction(int id)
    {
        var result = await _interactionService.DeleteInteractionAsync(id);
        if (!result)
            return NotFound($"Interaction with ID {id} not found");

        return NoContent();
    }

    [HttpPost("{id}/references")]
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

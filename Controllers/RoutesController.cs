using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>
    /// Get all routes
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var routes = await _routeService.GetAllAsync();
        return Ok(routes);
    }

    /// <summary>
    /// Get route by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var route = await _routeService.GetByIdAsync(id);

        if (route == null)
            return NotFound(new { message = "Route not found" });

        return Ok(route);
    }

    /// <summary>
    /// Create new route (SuperAdmin and Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] RouteInformation route)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message, id) = await _routeService.CreateAsync(route);
        
        if (!success)
            return BadRequest(new { message });

        return CreatedAtAction(nameof(GetById), new { id }, new { message, id });
    }

    /// <summary>
    /// Update route (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Update(int id, [FromBody] RouteInformation route)
    {
        if (id != route.Id)
            return BadRequest(new { message = "ID mismatch" });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, message) = await _routeService.UpdateAsync(id, route);
        
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }

    /// <summary>
    /// Delete route (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _routeService.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message });

        return Ok(new { message });
    }
}

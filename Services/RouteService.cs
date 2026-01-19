using Microsoft.EntityFrameworkCore;
using MedManagerApi.Data;
using MedManagerApi.Models;

namespace MedManagerApi.Services;

public class RouteService : IRouteService
{
    private readonly MedManagerDbContext _context;
    private readonly ILogger<RouteService> _logger;

    public RouteService(MedManagerDbContext context, ILogger<RouteService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RouteInformation>> GetAllAsync()
    {
        return await _context.RouteInformations
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<RouteInformation?> GetByIdAsync(int id)
    {
        return await _context.RouteInformations
            .Include(r => r.Drugs)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<(bool success, string message, int? id)> CreateAsync(RouteInformation route)
    {
        try
        {
            // Check if code already exists
            if (await _context.RouteInformations.AnyAsync(r => r.Code == route.Code))
                return (false, "Route code already exists", null);

            route.CreatedAt = DateTime.UtcNow;
            route.UpdatedAt = DateTime.UtcNow;

            _context.RouteInformations.Add(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Route created: {Code} - {Name}", route.Code, route.Name);

            return (true, "Route created successfully", route.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating route: {Code}", route.Code);
            return (false, "An error occurred while creating the route", null);
        }
    }

    public async Task<(bool success, string message)> UpdateAsync(int id, RouteInformation route)
    {
        try
        {
            var existing = await _context.RouteInformations.FindAsync(id);
            if (existing == null)
                return (false, "Route not found");

            // Check if code is being changed to one that already exists
            if (existing.Code != route.Code && 
                await _context.RouteInformations.AnyAsync(r => r.Code == route.Code))
                return (false, "Route code already exists");

            existing.Code = route.Code;
            existing.Name = route.Name;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Route updated: {Code} - {Name}", route.Code, route.Name);

            return (true, "Route updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating route: {Id}", id);
            return (false, "An error occurred while updating the route");
        }
    }

    public async Task<(bool success, string message)> DeleteAsync(int id)
    {
        try
        {
            var route = await _context.RouteInformations.FindAsync(id);
            if (route == null)
                return (false, "Route not found");

            // Check if route is used in any drugs
            var isUsed = await _context.Drugs.AnyAsync(d => d.RouteId == id);
            if (isUsed)
                return (false, "Cannot delete route that is used in drugs");

            _context.RouteInformations.Remove(route);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Route deleted: {Code} - {Name}", route.Code, route.Name);

            return (true, "Route deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting route: {Id}", id);
            return (false, "An error occurred while deleting the route");
        }
    }
}

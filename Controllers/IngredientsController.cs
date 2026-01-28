using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedManagerApi.Models;
using MedManagerApi.Services;

namespace MedManagerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;
    private readonly ISearchLogService _searchLogService;

    public IngredientsController(IIngredientService ingredientService, ISearchLogService searchLogService)
    {
        _ingredientService = ingredientService;
        _searchLogService = searchLogService;
    }

        /// <summary>
        /// Get all ingredients
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var (ingredients, totalCount, totalPages) = await _ingredientService.GetAllAsync(page, pageSize);

            return Ok(new
            {
                data = ingredients,
                pagination = new
                {
                    currentPage = page,
                    pageSize,
                    totalCount,
                    totalPages
                }
            });
        }

        /// <summary>
        /// Get ingredient by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ingredient = await _ingredientService.GetByIdAsync(id);

            if (ingredient == null)
                return NotFound(new { message = "Ingredient not found" });

            return Ok(ingredient);
        }

        /// <summary>
        /// Search ingredients by name or code
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(new { message = "Search query is required" });

            var ingredients = await _ingredientService.SearchAsync(q);

            // Log search
            var userId = User.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value : null;
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            await _searchLogService.LogSearchAsync(
                q,
                SearchEntityType.Ingredient,
                ingredients.Count,
                userId,
                ipAddress,
                userAgent
            );

            return Ok(ingredients);
        }

        /// <summary>
        /// Create new ingredient (SuperAdmin and Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
        public async Task<IActionResult> Create([FromBody] Ingredient ingredient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, id) = await _ingredientService.CreateAsync(ingredient);

            if (!success)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(GetById), new { id }, new { message, id });
        }

        /// <summary>
        /// Update ingredient (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ingredient ingredient)
        {
            if (id != ingredient.Id)
                return BadRequest(new { message = "ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message) = await _ingredientService.UpdateAsync(id, ingredient);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }

        /// <summary>
        /// Delete ingredient (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.SuperAdmin},{AppRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _ingredientService.DeleteAsync(id);

            if (!success)
                return NotFound(new { message });

            return Ok(new { message });
        }
    }

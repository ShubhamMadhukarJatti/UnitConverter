using Microsoft.AspNetCore.Mvc;
using UnitConverter.API.Models;
using UnitConverter.API.Services;

namespace UnitConverter.API.Controllers;

/// <summary>
/// Provides discovery endpoints for browsing supported units and categories.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class UnitsController : ControllerBase
{
    private readonly IConversionService _service;

    public UnitsController(IConversionService service)
    {
        _service = service;
    }

    /// <summary>
    /// List all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">
    /// Optional category name to filter by (e.g., "Length", "Temperature").
    /// Case-insensitive.
    /// </param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UnitInfo>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] string? category = null)
    {
        var units = _service.GetUnits(category);
        return Ok(units);
    }

    /// <summary>
    /// List all supported conversion categories.
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetCategories()
    {
        return Ok(_service.GetCategories());
    }
}

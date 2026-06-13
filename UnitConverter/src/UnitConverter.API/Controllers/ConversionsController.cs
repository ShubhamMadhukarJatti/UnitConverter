using Microsoft.AspNetCore.Mvc;
using UnitConverter.API.Models;
using UnitConverter.API.Services;

namespace UnitConverter.API.Controllers;

/// <summary>
/// Provides endpoints for converting values between units of measurement.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ConversionsController : ControllerBase
{
    private readonly IConversionService _service;

    public ConversionsController(IConversionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Convert a value from one unit to another.
    /// </summary>
    /// <remarks>
    /// Both units must belong to the same category (e.g., you cannot convert
    /// metres to kilograms). Unit symbols are case-sensitive.
    ///
    /// **Example:** Convert 100 °C to °F
    /// ```
    /// POST /api/conversions
    /// { "value": 100, "fromUnit": "°C", "toUnit": "°F" }
    /// ```
    /// </remarks>
    /// <param name="request">The conversion request.</param>
    /// <returns>The converted value with metadata.</returns>
    /// <response code="200">Conversion successful.</response>
    /// <response code="400">Invalid request (missing fields, incompatible units, etc.).</response>
    /// <response code="422">The unit symbols are not recognised or belong to different categories.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Post([FromBody] ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FromUnit) || string.IsNullOrWhiteSpace(request.ToUnit))
        {
            return BadRequest(new ErrorResponse
            {
                Code    = "MISSING_UNIT",
                Message = "Both 'fromUnit' and 'toUnit' must be provided.",
            });
        }

        var result = _service.Convert(request);

        if (result is null)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                Code    = "INCOMPATIBLE_UNITS",
                Message = $"Cannot convert from '{request.FromUnit}' to '{request.ToUnit}'. " +
                          "Either one or both unit symbols are unknown, or they belong to different categories. " +
                          "Use GET /api/units to browse available units.",
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Convert a value using query-string parameters (GET convenience endpoint).
    /// </summary>
    /// <remarks>
    /// Equivalent to the POST endpoint but useful for quick browser/curl testing.
    ///
    /// **Example:** `GET /api/conversions?value=1&amp;fromUnit=mi&amp;toUnit=km`
    /// </remarks>
    /// <param name="value">The numeric value to convert.</param>
    /// <param name="fromUnit">The source unit symbol.</param>
    /// <param name="toUnit">The target unit symbol.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public IActionResult Get(
        [FromQuery] double value,
        [FromQuery] string fromUnit,
        [FromQuery] string toUnit)
    {
        return Post(new ConversionRequest
        {
            Value    = value,
            FromUnit = fromUnit,
            ToUnit   = toUnit,
        });
    }
}

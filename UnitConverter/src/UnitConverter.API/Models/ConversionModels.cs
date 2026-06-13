namespace UnitConverter.API.Models;

/// <summary>
/// Represents a request to convert a value from one unit to another.
/// </summary>
public record ConversionRequest
{
    /// <summary>The numeric value to convert.</summary>
    /// <example>100</example>
    public required double Value { get; init; }

    /// <summary>The source unit symbol (e.g., "m", "kg", "°C").</summary>
    /// <example>m</example>
    public required string FromUnit { get; init; }

    /// <summary>The target unit symbol (e.g., "ft", "lb", "°F").</summary>
    /// <example>ft</example>
    public required string ToUnit { get; init; }
}

/// <summary>
/// Represents the result of a unit conversion.
/// </summary>
public record ConversionResult
{
    /// <summary>The original input value.</summary>
    public required double InputValue { get; init; }

    /// <summary>The source unit symbol.</summary>
    public required string FromUnit { get; init; }

    /// <summary>The converted output value.</summary>
    public required double OutputValue { get; init; }

    /// <summary>The target unit symbol.</summary>
    public required string ToUnit { get; init; }

    /// <summary>Human-readable label for the source unit.</summary>
    public required string FromUnitName { get; init; }

    /// <summary>Human-readable label for the target unit.</summary>
    public required string ToUnitName { get; init; }

    /// <summary>The conversion category (e.g., "Length", "Temperature").</summary>
    public required string Category { get; init; }
}

/// <summary>
/// Represents metadata about a supported unit.
/// </summary>
public record UnitInfo
{
    /// <summary>The unit symbol (e.g., "m", "kg").</summary>
    public required string Symbol { get; init; }

    /// <summary>The full name of the unit (e.g., "Metre").</summary>
    public required string Name { get; init; }

    /// <summary>The conversion category (e.g., "Length").</summary>
    public required string Category { get; init; }
}

/// <summary>
/// Represents an error response.
/// </summary>
public record ErrorResponse
{
    /// <summary>A short error code.</summary>
    public required string Code { get; init; }

    /// <summary>A human-readable error message.</summary>
    public required string Message { get; init; }
}

using UnitConverter.API.Models;

namespace UnitConverter.API.Services;

/// <summary>
/// Provides unit conversion operations.
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="request">The conversion request.</param>
    /// <returns>
    /// A <see cref="ConversionResult"/> on success, or <c>null</c> if the
    /// source/target unit symbols are unknown or belong to different categories.
    /// </returns>
    ConversionResult? Convert(ConversionRequest request);

    /// <summary>
    /// Returns metadata for all supported units, optionally filtered by category.
    /// </summary>
    IEnumerable<UnitInfo> GetUnits(string? category = null);

    /// <summary>
    /// Returns the distinct set of supported category names.
    /// </summary>
    IEnumerable<string> GetCategories();
}

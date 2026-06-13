using UnitConverter.API.Configuration;
using UnitConverter.API.Models;

namespace UnitConverter.API.Services;

/// <summary>
/// Converts between units using the definitions registered in <see cref="UnitRegistry"/>.
///
/// Conversion algorithm (two-step, via base unit):
///   1. to_base   = (value × fromFactor) + fromOffset
///   2. to_target = (to_base − toOffset) ÷ toFactor
///
/// This approach means adding a new unit only requires one factor (and an optional
/// offset for non-linear scales like temperature) — not an N×N matrix of pairs.
/// </summary>
public sealed class ConversionService : IConversionService
{
    // Flat look-up: symbol (case-sensitive) → (definition, category name)
    private readonly IReadOnlyDictionary<string, (UnitDefinition Def, string Category)> _index;

    public ConversionService()
    {
        var dict = new Dictionary<string, (UnitDefinition, string)>(StringComparer.Ordinal);

        foreach (var category in UnitRegistry.All)
        {
            foreach (var unit in category.Units)
            {
                // Last writer wins — avoids silent data bugs if a symbol is duplicated.
                dict[unit.Symbol] = (unit, category.Name);
            }
        }

        _index = dict;
    }

    /// <inheritdoc/>
    public ConversionResult? Convert(ConversionRequest request)
    {
        if (!_index.TryGetValue(request.FromUnit, out var from)) return null;
        if (!_index.TryGetValue(request.ToUnit,   out var to))   return null;

        // Units must belong to the same category.
        if (!string.Equals(from.Category, to.Category, StringComparison.Ordinal))
            return null;

        var baseValue  = request.Value * from.Def.Factor + from.Def.Offset;
        var outputValue = (baseValue - to.Def.Offset) / to.Def.Factor;

        return new ConversionResult
        {
            InputValue   = request.Value,
            FromUnit     = request.FromUnit,
            OutputValue  = outputValue,
            ToUnit       = request.ToUnit,
            FromUnitName = from.Def.Name,
            ToUnitName   = to.Def.Name,
            Category     = from.Category,
        };
    }

    /// <inheritdoc/>
    public IEnumerable<UnitInfo> GetUnits(string? category = null)
    {
        var query = _index.Values.AsEnumerable();

        if (category is not null)
        {
            query = query.Where(x =>
                string.Equals(x.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        return query
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Def.Name)
            .Select(x => new UnitInfo
            {
                Symbol   = x.Def.Symbol,
                Name     = x.Def.Name,
                Category = x.Category,
            });
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetCategories() =>
        UnitRegistry.All.Select(c => c.Name).Order();
}

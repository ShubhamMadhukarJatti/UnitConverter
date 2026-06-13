namespace UnitConverter.API.Configuration;

/// <summary>
/// Represents a unit of measurement with its conversion factor relative to a base unit.
/// For linear conversions: value_in_base = (value * Factor) + Offset
/// For inverse conversions: value_in_base = Factor / value  (IsInverse = true, rare)
/// </summary>
public record UnitDefinition
{
    public required string Symbol { get; init; }
    public required string Name { get; init; }

    /// <summary>
    /// Multiplier to convert this unit to the category's base unit.
    /// e.g., 1 inch = 0.0254 metres, so Factor = 0.0254 for "in" when base is "m".
    /// </summary>
    public double Factor { get; init; } = 1.0;

    /// <summary>
    /// Additive offset applied AFTER multiplication when converting to base.
    /// Only non-zero for temperature scales.
    /// </summary>
    public double Offset { get; init; } = 0.0;
}

/// <summary>
/// Describes a category of related units and the strategy for converting between them.
/// </summary>
public record UnitCategory
{
    public required string Name { get; init; }
    public required IReadOnlyList<UnitDefinition> Units { get; init; }
}

/// <summary>
/// Central registry of all supported unit categories and their conversion data.
/// To add a new unit: add a <see cref="UnitDefinition"/> to the relevant category.
/// To add a new category: add a new <see cref="UnitCategory"/> to <see cref="All"/>.
/// </summary>
public static class UnitRegistry
{
    // ── Length (base: metre) ────────────────────────────────────────────────────
    private static readonly UnitCategory Length = new()
    {
        Name = "Length",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "m",   Name = "Metre",       Factor = 1.0 },
            new() { Symbol = "km",  Name = "Kilometre",   Factor = 1_000.0 },
            new() { Symbol = "cm",  Name = "Centimetre",  Factor = 0.01 },
            new() { Symbol = "mm",  Name = "Millimetre",  Factor = 0.001 },
            new() { Symbol = "um",  Name = "Micrometre",  Factor = 1e-6 },
            new() { Symbol = "nm",  Name = "Nanometre",   Factor = 1e-9 },
            new() { Symbol = "in",  Name = "Inch",        Factor = 0.0254 },
            new() { Symbol = "ft",  Name = "Foot",        Factor = 0.3048 },
            new() { Symbol = "yd",  Name = "Yard",        Factor = 0.9144 },
            new() { Symbol = "mi",  Name = "Mile",        Factor = 1_609.344 },
            new() { Symbol = "nmi", Name = "Nautical Mile", Factor = 1_852.0 },
            new() { Symbol = "ly",  Name = "Light-Year",  Factor = 9.4607304725808e15 },
        }
    };

    // ── Mass / Weight (base: kilogram) ──────────────────────────────────────────
    private static readonly UnitCategory Mass = new()
    {
        Name = "Mass",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "kg",  Name = "Kilogram",   Factor = 1.0 },
            new() { Symbol = "g",   Name = "Gram",       Factor = 0.001 },
            new() { Symbol = "mg",  Name = "Milligram",  Factor = 1e-6 },
            new() { Symbol = "t",   Name = "Metric Ton", Factor = 1_000.0 },
            new() { Symbol = "lb",  Name = "Pound",      Factor = 0.45359237 },
            new() { Symbol = "oz",  Name = "Ounce",      Factor = 0.028349523125 },
            new() { Symbol = "st",  Name = "Stone",      Factor = 6.35029318 },
            new() { Symbol = "ton", Name = "Short Ton",  Factor = 907.18474 },
            new() { Symbol = "LT",  Name = "Long Ton",   Factor = 1016.0469088 },
        }
    };

    // ── Temperature ─────────────────────────────────────────────────────────────
    // Base unit: Kelvin.
    // toBase(x) = x * Factor + Offset
    // fromBase(k) = (k - Offset) / Factor
    private static readonly UnitCategory Temperature = new()
    {
        Name = "Temperature",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "K",  Name = "Kelvin",     Factor = 1.0,       Offset = 0.0 },
            new() { Symbol = "°C", Name = "Celsius",    Factor = 1.0,       Offset = 273.15 },
            new() { Symbol = "°F", Name = "Fahrenheit", Factor = 5.0 / 9.0, Offset = 255.3722222 },
            new() { Symbol = "°R", Name = "Rankine",    Factor = 5.0 / 9.0, Offset = 0.0 },
        }
    };

    // ── Area (base: square metre) ───────────────────────────────────────────────
    private static readonly UnitCategory Area = new()
    {
        Name = "Area",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "m2",   Name = "Square Metre",      Factor = 1.0 },
            new() { Symbol = "km2",  Name = "Square Kilometre",  Factor = 1e6 },
            new() { Symbol = "cm2",  Name = "Square Centimetre", Factor = 1e-4 },
            new() { Symbol = "mm2",  Name = "Square Millimetre", Factor = 1e-6 },
            new() { Symbol = "ha",   Name = "Hectare",           Factor = 10_000.0 },
            new() { Symbol = "ac",   Name = "Acre",              Factor = 4046.8564224 },
            new() { Symbol = "ft2",  Name = "Square Foot",       Factor = 0.09290304 },
            new() { Symbol = "in2",  Name = "Square Inch",       Factor = 6.4516e-4 },
            new() { Symbol = "mi2",  Name = "Square Mile",       Factor = 2589988.110336 },
            new() { Symbol = "yd2",  Name = "Square Yard",       Factor = 0.83612736 },
        }
    };

    // ── Volume (base: cubic metre) ──────────────────────────────────────────────
    private static readonly UnitCategory Volume = new()
    {
        Name = "Volume",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "m3",   Name = "Cubic Metre",   Factor = 1.0 },
            new() { Symbol = "L",    Name = "Litre",         Factor = 0.001 },
            new() { Symbol = "mL",   Name = "Millilitre",    Factor = 1e-6 },
            new() { Symbol = "cm3",  Name = "Cubic Centimetre", Factor = 1e-6 },
            new() { Symbol = "ft3",  Name = "Cubic Foot",    Factor = 0.028316846592 },
            new() { Symbol = "in3",  Name = "Cubic Inch",    Factor = 1.6387064e-5 },
            new() { Symbol = "gal",  Name = "US Gallon",     Factor = 0.003785411784 },
            new() { Symbol = "qt",   Name = "US Quart",      Factor = 9.46352946e-4 },
            new() { Symbol = "pt",   Name = "US Pint",       Factor = 4.73176473e-4 },
            new() { Symbol = "floz", Name = "US Fluid Ounce", Factor = 2.95735295625e-5 },
            new() { Symbol = "ukgal", Name = "Imperial Gallon", Factor = 0.00454609 },
        }
    };

    // ── Speed (base: metres per second) ─────────────────────────────────────────
    private static readonly UnitCategory Speed = new()
    {
        Name = "Speed",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "m/s",  Name = "Metres per Second",   Factor = 1.0 },
            new() { Symbol = "km/h", Name = "Kilometres per Hour", Factor = 1.0 / 3.6 },
            new() { Symbol = "mph",  Name = "Miles per Hour",      Factor = 0.44704 },
            new() { Symbol = "kn",   Name = "Knot",                Factor = 0.514444 },
            new() { Symbol = "ft/s", Name = "Feet per Second",     Factor = 0.3048 },
            new() { Symbol = "mach", Name = "Mach (ISA sea level)", Factor = 340.29 },
        }
    };

    // ── Energy (base: joule) ────────────────────────────────────────────────────
    private static readonly UnitCategory Energy = new()
    {
        Name = "Energy",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "J",    Name = "Joule",               Factor = 1.0 },
            new() { Symbol = "kJ",   Name = "Kilojoule",           Factor = 1_000.0 },
            new() { Symbol = "MJ",   Name = "Megajoule",           Factor = 1e6 },
            new() { Symbol = "cal",  Name = "Calorie",             Factor = 4.184 },
            new() { Symbol = "kcal", Name = "Kilocalorie",         Factor = 4_184.0 },
            new() { Symbol = "Wh",   Name = "Watt-Hour",           Factor = 3_600.0 },
            new() { Symbol = "kWh",  Name = "Kilowatt-Hour",       Factor = 3_600_000.0 },
            new() { Symbol = "BTU",  Name = "British Thermal Unit", Factor = 1055.05585262 },
            new() { Symbol = "eV",   Name = "Electronvolt",        Factor = 1.602176634e-19 },
        }
    };

    // ── Pressure (base: pascal) ─────────────────────────────────────────────────
    private static readonly UnitCategory Pressure = new()
    {
        Name = "Pressure",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "Pa",   Name = "Pascal",           Factor = 1.0 },
            new() { Symbol = "kPa",  Name = "Kilopascal",       Factor = 1_000.0 },
            new() { Symbol = "MPa",  Name = "Megapascal",       Factor = 1e6 },
            new() { Symbol = "bar",  Name = "Bar",              Factor = 100_000.0 },
            new() { Symbol = "mbar", Name = "Millibar",         Factor = 100.0 },
            new() { Symbol = "atm",  Name = "Atmosphere",       Factor = 101_325.0 },
            new() { Symbol = "psi",  Name = "Pounds per Square Inch", Factor = 6894.757293168 },
            new() { Symbol = "mmHg", Name = "Millimetre of Mercury", Factor = 133.322387415 },
            new() { Symbol = "inHg", Name = "Inch of Mercury",  Factor = 3386.389 },
        }
    };

    // ── Digital Storage (base: byte) ────────────────────────────────────────────
    private static readonly UnitCategory DigitalStorage = new()
    {
        Name = "Digital Storage",
        Units = new List<UnitDefinition>
        {
            new() { Symbol = "bit",  Name = "Bit",      Factor = 0.125 },
            new() { Symbol = "B",    Name = "Byte",     Factor = 1.0 },
            new() { Symbol = "KB",   Name = "Kilobyte", Factor = 1_000.0 },
            new() { Symbol = "MB",   Name = "Megabyte", Factor = 1e6 },
            new() { Symbol = "GB",   Name = "Gigabyte", Factor = 1e9 },
            new() { Symbol = "TB",   Name = "Terabyte", Factor = 1e12 },
            new() { Symbol = "PB",   Name = "Petabyte", Factor = 1e15 },
            new() { Symbol = "KiB",  Name = "Kibibyte", Factor = 1024.0 },
            new() { Symbol = "MiB",  Name = "Mebibyte", Factor = 1048576.0 },
            new() { Symbol = "GiB",  Name = "Gibibyte", Factor = 1073741824.0 },
            new() { Symbol = "TiB",  Name = "Tebibyte", Factor = 1099511627776.0 },
        }
    };

    /// <summary>
    /// All registered unit categories. Add new categories here.
    /// </summary>
    public static readonly IReadOnlyList<UnitCategory> All = new List<UnitCategory>
    {
        Length,
        Mass,
        Temperature,
        Area,
        Volume,
        Speed,
        Energy,
        Pressure,
        DigitalStorage,
    };
}

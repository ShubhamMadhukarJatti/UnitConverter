using FluentAssertions;
using UnitConverter.API.Models;
using UnitConverter.API.Services;
using Xunit;

namespace UnitConverter.Tests.Services;

public sealed class ConversionServiceTests
{
    private readonly ConversionService _sut = new();

    // ── Length ─────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1.0,    "m",  "ft",  3.28084)]
    [InlineData(1.0,    "km", "mi",  0.621371)]
    [InlineData(1.0,    "in", "cm",  2.54)]
    [InlineData(1_000.0,"m",  "km",  1.0)]
    public void Convert_Length_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value    = input,
            FromUnit = from,
            ToUnit   = to,
        });

        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(expected, precision: 0.0001);
        result.Category.Should().Be("Length");
    }

    // ── Temperature ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0.0,    "°C", "°F",  32.0)]
    [InlineData(100.0,  "°C", "°F",  212.0)]
    [InlineData(32.0,   "°F", "°C",  0.0)]
    [InlineData(0.0,    "°C", "K",   273.15)]
    [InlineData(0.0,    "K",  "°C", -273.15)]
    public void Convert_Temperature_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value    = input,
            FromUnit = from,
            ToUnit   = to,
        });

        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(expected, precision: 0.001);
        result.Category.Should().Be("Temperature");
    }

    // ── Mass ───────────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1.0,  "kg", "lb",  2.20462)]
    [InlineData(1.0,  "lb", "kg",  0.453592)]
    [InlineData(1000.0,"g", "kg",  1.0)]
    public void Convert_Mass_ReturnsCorrectResult(
        double input, string from, string to, double expected)
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value    = input,
            FromUnit = from,
            ToUnit   = to,
        });

        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(expected, precision: 0.0001);
        result.Category.Should().Be("Mass");
    }

    // ── Identity conversions ───────────────────────────────────────────────────

    [Theory]
    [InlineData(42.0, "m")]
    [InlineData(42.0, "kg")]
    [InlineData(42.0, "°C")]
    public void Convert_SameUnit_ReturnsInputValue(double value, string unit)
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value    = value,
            FromUnit = unit,
            ToUnit   = unit,
        });

        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(value, precision: 1e-10);
    }

    // ── Error cases ────────────────────────────────────────────────────────────

    [Fact]
    public void Convert_UnknownFromUnit_ReturnsNull()
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value = 1, FromUnit = "UNKNOWN", ToUnit = "m",
        });

        result.Should().BeNull();
    }

    [Fact]
    public void Convert_UnknownToUnit_ReturnsNull()
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value = 1, FromUnit = "m", ToUnit = "UNKNOWN",
        });

        result.Should().BeNull();
    }

    [Fact]
    public void Convert_IncompatibleCategories_ReturnsNull()
    {
        var result = _sut.Convert(new ConversionRequest
        {
            Value = 1, FromUnit = "m", ToUnit = "kg",
        });

        result.Should().BeNull();
    }

    // ── GetUnits / GetCategories ───────────────────────────────────────────────

    [Fact]
    public void GetUnits_NoFilter_ReturnsAllUnits()
    {
        var units = _sut.GetUnits().ToList();

        units.Should().NotBeEmpty();
        units.Select(u => u.Symbol).Should().Contain(new[] { "m", "kg", "°C", "L", "J" });
    }

    [Fact]
    public void GetUnits_CategoryFilter_ReturnsOnlyThatCategory()
    {
        var units = _sut.GetUnits("Temperature").ToList();

        units.Should().OnlyContain(u => u.Category == "Temperature");
        units.Select(u => u.Symbol).Should().Contain(new[] { "°C", "°F", "K" });
    }

    [Fact]
    public void GetCategories_ReturnsExpectedCategories()
    {
        var categories = _sut.GetCategories().ToList();

        categories.Should().Contain(new[]
        {
            "Length", "Mass", "Temperature", "Area", "Volume",
            "Speed", "Energy", "Pressure", "Digital Storage",
        });
    }
}

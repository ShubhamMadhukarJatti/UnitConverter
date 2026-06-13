using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConverter.API.Models;
using Xunit;

namespace UnitConverter.Tests.Controllers;

/// <summary>
/// Integration tests that spin up the full ASP.NET Core pipeline in memory.
/// </summary>
public sealed class ConversionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConversionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ── POST /api/conversions ─────────────────────────────────────────────────

    [Fact]
    public async Task Post_ValidConversion_Returns200WithResult()
    {
        var response = await _client.PostAsJsonAsync("/api/conversions", new
        {
            value    = 100,
            fromUnit = "°C",
            toUnit   = "°F",
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result.Should().NotBeNull();
        result!.OutputValue.Should().BeApproximately(212.0, precision: 0.01);
        result.Category.Should().Be("Temperature");
    }

    [Fact]
    public async Task Post_IncompatibleUnits_Returns422()
    {
        var response = await _client.PostAsJsonAsync("/api/conversions", new
        {
            value    = 1,
            fromUnit = "m",
            toUnit   = "kg",
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error!.Code.Should().Be("INCOMPATIBLE_UNITS");
    }

    [Fact]
    public async Task Post_UnknownUnit_Returns422()
    {
        var response = await _client.PostAsJsonAsync("/api/conversions", new
        {
            value    = 1,
            fromUnit = "parsecs",
            toUnit   = "m",
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── GET /api/conversions ──────────────────────────────────────────────────

    [Fact]
    public async Task Get_ValidQueryString_Returns200()
    {
        var response = await _client.GetAsync(
            "/api/conversions?value=1&fromUnit=mi&toUnit=km");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<ConversionResult>();
        result!.OutputValue.Should().BeApproximately(1.60934, precision: 0.0001);
    }

    // ── GET /api/units ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetUnits_NoFilter_ReturnsAllUnits()
    {
        var response = await _client.GetAsync("/api/units");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var units = await response.Content.ReadFromJsonAsync<List<UnitInfo>>();
        units.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetUnits_CategoryFilter_ReturnsFilteredUnits()
    {
        var response = await _client.GetAsync("/api/units?category=Length");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var units = await response.Content.ReadFromJsonAsync<List<UnitInfo>>();
        units.Should().OnlyContain(u => u.Category == "Length");
    }

    // ── GET /api/units/categories ─────────────────────────────────────────────

    [Fact]
    public async Task GetCategories_ReturnsExpectedCategories()
    {
        var response = await _client.GetAsync("/api/units/categories");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var categories = await response.Content.ReadFromJsonAsync<List<string>>();
        categories.Should().Contain(new[] { "Length", "Mass", "Temperature" });
    }
}

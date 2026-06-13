# Unit Converter API

A RESTful ASP.NET Core 8 Web API for converting numerical values between units of measurement.

## Supported categories

| Category         | Example units                              |
|------------------|--------------------------------------------|
| Length           | m, km, cm, mm, in, ft, yd, mi, nmi, ly    |
| Mass             | kg, g, mg, t, lb, oz, st, ton, LT         |
| Temperature      | °C, °F, K, °R                             |
| Area             | m2, km2, ha, ac, ft2, in2, mi2, yd2       |
| Volume           | m3, L, mL, gal, qt, pt, floz, ukgal       |
| Speed            | m/s, km/h, mph, kn, ft/s, mach            |
| Energy           | J, kJ, MJ, cal, kcal, Wh, kWh, BTU, eV   |
| Pressure         | Pa, kPa, bar, atm, psi, mmHg, inHg        |
| Digital Storage  | bit, B, KB, MB, GB, TB, PB, KiB, MiB, GiB |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later

Verify your installation:

```bash
dotnet --version   # should print 8.x.x or higher
```

---

## Running locally

```bash
# 1. Clone
git clone <repo-url>
cd UnitConverter

# 2. Run
dotnet run --project src/UnitConverter.API

# The API starts on https://localhost:5001 and http://localhost:5000
# Swagger UI is available at http://localhost:5000
```

### Running with Docker (optional)

```bash
docker build -t unit-converter .
docker run -p 8080:8080 unit-converter
# API is now at http://localhost:8080
```

---

## API endpoints

### `POST /api/conversions`

Convert a value between units.

**Request body**

```json
{
  "value": 100,
  "fromUnit": "°C",
  "toUnit": "°F"
}
```

**Response `200 OK`**

```json
{
  "inputValue": 100,
  "fromUnit": "°C",
  "outputValue": 212,
  "toUnit": "°F",
  "fromUnitName": "Celsius",
  "toUnitName": "Fahrenheit",
  "category": "Temperature"
}
```

**Response `422 Unprocessable Entity`** — unknown or incompatible units

```json
{
  "code": "INCOMPATIBLE_UNITS",
  "message": "Cannot convert from 'm' to 'kg'. ..."
}
```

---

### `GET /api/conversions`

Convenience GET endpoint (query-string parameters).

```
GET /api/conversions?value=1&fromUnit=mi&toUnit=km
```

---

### `GET /api/units`

List all supported units. Filter by category with `?category=Length`.

```
GET /api/units
GET /api/units?category=Temperature
```

---

### `GET /api/units/categories`

List all supported category names.

```
GET /api/units/categories
```

---

## Running tests

```bash
dotnet test
```

To see a coverage report (requires `coverlet`):

```bash
dotnet test --collect:"XPlat Code Coverage"
```

---

## Design decisions

### Two-step base-unit conversion

Every unit is defined by a single `Factor` and optional `Offset` relative to a **base unit** for its category (e.g., the metre for length, the kelvin for temperature).

```
toBase(x)   = x × factor + offset
fromBase(k) = (k − offset) ÷ factor
```

Converting A → B always goes through the base:

```
result = fromBase(toBase(input))
```

This means adding a new unit only requires **one row** — not an N × N lookup table of every pair. The approach handles non-linear scales (temperature) cleanly through the offset term.

### Case-sensitive unit symbols

Unit symbols are matched exactly as provided (e.g., `m` ≠ `M`, `°C` ≠ `c`). This avoids silent mismatches between symbols like `t` (metric ton) and `T` (tesla if pressure were extended), and aligns with SI/ISO convention.

### Singleton service

`ConversionService` is registered as a singleton. It builds its unit index once at startup — O(1) lookups thereafter — and holds no mutable state, so there are no concurrency concerns.

### Separation of registry and service

`UnitRegistry` (static data) is kept separate from `ConversionService` (business logic). A future version could load `UnitRegistry` data from a database or configuration file without touching the conversion algorithm.

### Extensibility

To add a new unit: add a `UnitDefinition` to the relevant category in `UnitRegistry.cs`.  
To add a new category: add a new `UnitCategory` entry to `UnitRegistry.All`.  
No other code changes are required.

### Error handling

- `400 Bad Request` — structurally invalid payload (e.g., missing required fields).  
- `422 Unprocessable Entity` — payload is valid JSON but the conversion cannot be performed (unknown symbol, different categories). Using 422 rather than 400 distinguishes a client logic error from a protocol error.

---

## Project structure

```
UnitConverter/
├── src/
│   └── UnitConverter.API/
│       ├── Configuration/
│       │   └── UnitRegistry.cs        # All unit definitions (data layer)
│       ├── Controllers/
│       │   ├── ConversionsController.cs
│       │   └── UnitsController.cs
│       ├── Models/
│       │   └── ConversionModels.cs    # Request / response records
│       ├── Services/
│       │   ├── IConversionService.cs
│       │   └── ConversionService.cs   # Conversion logic
│       ├── Program.cs
│       └── appsettings*.json
└── tests/
    └── UnitConverter.Tests/
        ├── Controllers/
        │   └── ConversionsControllerTests.cs   # Integration tests
        └── Services/
            └── ConversionServiceTests.cs        # Unit tests
```

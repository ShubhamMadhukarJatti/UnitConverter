using System.Reflection;
using UnitConverter.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// Register the conversion service as a singleton: it is stateless and its
// internal unit index is built once at startup.
builder.Services.AddSingleton<IConversionService, ConversionService>();

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title       = "Unit Converter API",
        Version     = "v1",
        Description = "Convert numerical values between units of measurement across " +
                      "categories such as length, mass, temperature, area, volume, " +
                      "speed, energy, pressure, and digital storage.",
    });

    // Include XML doc comments in Swagger UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ── CORS (permissive for local development) ───────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Converter API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root /
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Expose for integration tests.
public partial class Program { }

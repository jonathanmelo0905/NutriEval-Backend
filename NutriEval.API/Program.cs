using FluentValidation;
using FluentValidation.AspNetCore;
using NutriEval.API.Extensions;
using NutriEval.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Mapea las variables de entorno de Railway al sistema de configuración de ASP.NET Core
MapEnvironmentVariables(builder.Configuration);

// ── Servicios ────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddCloudinary(builder.Configuration);
builder.Services.AddApplicationServices();

// ── Pipeline ─────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseGlobalExceptionHandler();          // 1. captura toda excepción no controlada

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("NutriEvalPolicy");
app.UseAuthentication();                  // 2. valida JWT y puebla HttpContext.User
app.UseTenantContext();                   // 3. extrae tenant_id del claim y lo pone en Items
app.UseAuthorization();
app.MapControllers();

app.Run();

// ── Helpers ──────────────────────────────────────────────────────────────────
static void MapEnvironmentVariables(ConfigurationManager configuration)
{
    var mappings = new Dictionary<string, string>
    {
        ["DATABASE_URL"]          = "ConnectionStrings:DefaultConnection",
        ["JWT_SECRET"]            = "Jwt:Secret",
        ["JWT_ISSUER"]            = "Jwt:Issuer",
        ["JWT_AUDIENCE"]          = "Jwt:Audience",
        ["JWT_EXPIRES_HOURS"]     = "Jwt:ExpiresHours",
        ["CLOUDINARY_CLOUD_NAME"] = "Cloudinary:CloudName",
        ["CLOUDINARY_API_KEY"]    = "Cloudinary:ApiKey",
        ["CLOUDINARY_API_SECRET"] = "Cloudinary:ApiSecret",
        ["FRONTEND_URL"]          = "Cors:FrontendUrl"
    };

    foreach (var (envVar, configKey) in mappings)
    {
        var value = Environment.GetEnvironmentVariable(envVar);
        if (!string.IsNullOrEmpty(value))
            configuration[configKey] = value;
    }

    // Railway inyecta PORT — configurar Kestrel para escuchar en ese puerto
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
        configuration["Kestrel:Endpoints:Http:Url"] = $"http://+:{port}";
}

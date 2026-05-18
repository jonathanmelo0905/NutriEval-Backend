using FluentValidation;
using FluentValidation.AspNetCore;
using Npgsql;
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
        if (string.IsNullOrEmpty(value)) continue;

        configuration[configKey] = envVar == "DATABASE_URL"
            ? ConvertirDatabaseUrl(value)
            : value;
    }

    // Railway inyecta PORT — configurar Kestrel para escuchar en ese puerto
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
        configuration["Kestrel:Endpoints:Http:Url"] = $"http://+:{port}";
}

// Convierte postgresql://user:pass@host:port/db[?params] al formato key=value de Npgsql.
// Si el valor ya está en formato key=value lo devuelve sin modificar.
static string ConvertirDatabaseUrl(string url)
{
    if (!url.StartsWith("postgresql://") && !url.StartsWith("postgres://"))
        return url;

    var uri    = new Uri(url);
    var partes = uri.UserInfo.Split(':', 2);

    var builder = new NpgsqlConnectionStringBuilder
    {
        Host     = uri.Host,
        Port     = uri.Port > 0 ? uri.Port : 5432,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = Uri.UnescapeDataString(partes[0]),
        Password = partes.Length > 1 ? Uri.UnescapeDataString(partes[1]) : string.Empty,
    };

    // Propagar parámetros de query (Railway público añade ?sslmode=require)
    if (!string.IsNullOrEmpty(uri.Query))
    {
        foreach (var param in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = param.Split('=', 2);
            if (kv.Length != 2) continue;

            if (kv[0].Equals("sslmode", StringComparison.OrdinalIgnoreCase) &&
                Enum.TryParse<SslMode>(kv[1], ignoreCase: true, out var sslMode))
            {
                builder.SslMode = sslMode;
            }
        }
    }

    return builder.ConnectionString;
}

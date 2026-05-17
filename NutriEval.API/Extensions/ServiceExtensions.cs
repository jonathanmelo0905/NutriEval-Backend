using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NutriEval.API.Data;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories;
using NutriEval.API.Repositories.Interfaces;
using NutriEval.API.Services;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "La cadena de conexión 'DefaultConnection' no está configurada. " +
                "Define DATABASE_URL como variable de entorno.");

        services.AddDbContext<NutriEvalDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "JWT_SECRET no está configurado. Define la variable de entorno.");

        var issuer   = configuration["Jwt:Issuer"]   ?? "nutrieval-api";
        var audience = configuration["Jwt:Audience"] ?? "nutrieval-app";
        var key      = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = key,
                    ValidateIssuer           = true,
                    ValidIssuer              = issuer,
                    ValidateAudience         = true,
                    ValidAudience            = audience,
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services, IConfiguration configuration)
    {
        var configuredOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        var frontendUrl = configuration["Cors:FrontendUrl"];

        var allOrigins = string.IsNullOrEmpty(frontendUrl)
            ? configuredOrigins
            : configuredOrigins.Union([frontendUrl]).ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy("NutriEvalPolicy", policy =>
            {
                policy.WithOrigins(allOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title       = "NutriEval API",
                Version     = "v1",
                Description = "Backend REST para la plataforma SaaS NutriEval"
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name        = "Authorization",
                Description = "JWT Authorization header. Formato: Bearer {token}",
                In          = ParameterLocation.Header,
                Type        = SecuritySchemeType.Http,
                Scheme      = "Bearer",
                BearerFormat = "JWT",
                Reference   = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = JwtBearerDefaults.AuthenticationScheme
                }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { scheme, Array.Empty<string>() }
            });
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Entrenador>, PasswordHasher<Entrenador>>();
        services.AddScoped<IPasswordHasher<Cliente>, PasswordHasher<Cliente>>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IClienteService, ClienteService>();

        return services;
    }
}

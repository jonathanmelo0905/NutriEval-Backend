using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NutriEval.API.Data;
using NutriEval.API.Models.DTOs.Auth;
using NutriEval.API.Models.Entities;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class AuthService(
    NutriEvalDbContext db,
    IPasswordHasher<Entrenador> entrenadorHasher,
    IPasswordHasher<Cliente> clienteHasher,
    IConfiguration configuration,
    ILogger<AuthService> logger) : IAuthService
{
    private string Secret     => configuration["Jwt:Secret"]!;
    private string Issuer     => configuration["Jwt:Issuer"]     ?? "nutrieval-api";
    private string Audience   => configuration["Jwt:Audience"]   ?? "nutrieval-app";
    private int    AccessHours => int.TryParse(configuration["Jwt:ExpiresHours"], out var h) ? h : 24;

    // ── Registro ─────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RegisterEntrenadorAsync(RegisterEntrenadorDto dto)
    {
        var email = dto.Email.ToLower().Trim();

        if (await db.Entrenadores.AnyAsync(e => e.Email == email))
            throw new ArgumentException("El email ya está registrado.");

        var entrenador = new Entrenador
        {
            Id           = Guid.NewGuid(),
            Nombre       = dto.Nombre.Trim(),
            Email        = email,
            Plan         = "trial",
            TrialEndsAt  = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14))
        };

        entrenador.PasswordHash = entrenadorHasher.HashPassword(entrenador, dto.Password);

        db.Entrenadores.Add(entrenador);
        await db.SaveChangesAsync();

        logger.LogInformation("Entrenador registrado: {Email}", email);

        return BuildResponse(entrenador);
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.ToLower().Trim();

        var entrenador = await db.Entrenadores
            .FirstOrDefaultAsync(e => e.Email == email && e.Activo);

        if (entrenador is not null)
        {
            if (entrenadorHasher.VerifyHashedPassword(entrenador, entrenador.PasswordHash, dto.Password)
                == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            logger.LogInformation("Login entrenador: {Email}", email);
            return BuildResponse(entrenador);
        }

        var cliente = await db.Clientes
            .FirstOrDefaultAsync(c => c.Email == email && c.Activo && c.PasswordHash != null);

        if (cliente is not null)
        {
            if (clienteHasher.VerifyHashedPassword(cliente, cliente.PasswordHash!, dto.Password)
                == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            logger.LogInformation("Login cliente: {Email}", email);
            return BuildResponse(cliente);
        }

        throw new UnauthorizedAccessException("Credenciales inválidas.");
    }

    // ── Refresh ───────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto dto)
    {
        var principal = ValidateRefreshToken(dto.RefreshToken);

        var tipo      = principal.FindFirstValue("tipo")    ?? throw new UnauthorizedAccessException("Token inválido.");
        var userIdStr = principal.FindFirstValue("user_id") ?? throw new UnauthorizedAccessException("Token inválido.");

        if (!Guid.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException("Token inválido.");

        if (tipo == "entrenador")
        {
            var entrenador = await db.Entrenadores.FindAsync(userId)
                ?? throw new KeyNotFoundException("Entrenador no encontrado.");

            if (!entrenador.Activo)
                throw new UnauthorizedAccessException("Cuenta desactivada.");

            return BuildResponse(entrenador);
        }
        else
        {
            var cliente = await db.Clientes.FindAsync(userId)
                ?? throw new KeyNotFoundException("Cliente no encontrado.");

            if (!cliente.Activo)
                throw new UnauthorizedAccessException("Cuenta desactivada.");

            return BuildResponse(cliente);
        }
    }

    // ── Me ────────────────────────────────────────────────────────────────────

    public async Task<MeDto> GetMeAsync(Guid userId, string tipo)
    {
        if (tipo == "entrenador")
        {
            var e = await db.Entrenadores.FindAsync(userId)
                ?? throw new KeyNotFoundException("Entrenador no encontrado.");

            return new MeDto
            {
                Id       = e.Id,
                TenantId = e.Id,
                Nombre   = e.Nombre,
                Email    = e.Email,
                Tipo     = "entrenador",
                Plan     = e.Plan
            };
        }
        else
        {
            var c = await db.Clientes.FindAsync(userId)
                ?? throw new KeyNotFoundException("Cliente no encontrado.");

            return new MeDto
            {
                Id       = c.Id,
                TenantId = c.TenantId,
                Nombre   = c.Nombre,
                Email    = c.Email ?? string.Empty,
                Tipo     = "cliente"
            };
        }
    }

    // ── Helpers privados ──────────────────────────────────────────────────────

    private AuthResponseDto BuildResponse(Entrenador e) => new()
    {
        AccessToken  = GenerateToken(e.Id, e.Id, e.Email, e.Nombre, "entrenador", AccessHours),
        RefreshToken = GenerateToken(e.Id, e.Id, e.Email, e.Nombre, "entrenador", hours: 24 * 7, isRefresh: true),
        Tipo         = "entrenador",
        Usuario      = new UsuarioInfoDto { Id = e.Id, TenantId = e.Id, Nombre = e.Nombre, Email = e.Email, Plan = e.Plan }
    };

    private AuthResponseDto BuildResponse(Cliente c) => new()
    {
        AccessToken  = GenerateToken(c.TenantId, c.Id, c.Email ?? string.Empty, c.Nombre, "cliente", AccessHours),
        RefreshToken = GenerateToken(c.TenantId, c.Id, c.Email ?? string.Empty, c.Nombre, "cliente", hours: 24 * 7, isRefresh: true),
        Tipo         = "cliente",
        Usuario      = new UsuarioInfoDto { Id = c.Id, TenantId = c.TenantId, Nombre = c.Nombre, Email = c.Email ?? string.Empty }
    };

    private string GenerateToken(
        Guid tenantId, Guid userId, string email, string nombre,
        string tipo, int hours, bool isRefresh = false)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, tenantId.ToString()),   // leído por TenantMiddleware
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("user_id", userId.ToString()),
            new("tipo",    tipo),
            new(ClaimTypes.Email, email),
            new("nombre",  nombre)
        };

        if (isRefresh)
            claims.Add(new Claim("token_type", "refresh"));

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:            Issuer,
            audience:          Audience,
            claims:            claims,
            expires:           DateTime.UtcNow.AddHours(hours),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal ValidateRefreshToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = key,
                ValidateIssuer           = true,
                ValidIssuer              = Issuer,
                ValidateAudience         = true,
                ValidAudience            = Audience,
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero
            }, out _);

            if (principal.FindFirstValue("token_type") != "refresh")
                throw new UnauthorizedAccessException("El token no es un refresh token.");

            return principal;
        }
        catch (SecurityTokenException)
        {
            throw new UnauthorizedAccessException("Refresh token inválido o expirado.");
        }
    }
}

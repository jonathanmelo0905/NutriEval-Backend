using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NutriEval.API.Data;
using NutriEval.API.Models.DTOs.Configuracion;
using NutriEval.API.Models.Entities;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class ConfiguracionService(
    NutriEvalDbContext db,
    ILogger<ConfiguracionService> logger) : IConfiguracionService
{
    public async Task<ConfiguracionDto> GetAsync(Guid tenantId)
    {
        var entrenador = await FindEntrenadorAsync(tenantId);
        return ToDto(entrenador);
    }

    public async Task<ConfiguracionDto> UpdateRedesAsync(Guid tenantId, UpdateRedesDto dto)
    {
        var entrenador = await FindEntrenadorAsync(tenantId);

        entrenador.RedesSociales = JsonSerializer.Serialize(dto);
        await db.SaveChangesAsync();

        logger.LogInformation("Redes sociales actualizadas para tenant {TenantId}", tenantId);

        return ToDto(entrenador);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<Entrenador> FindEntrenadorAsync(Guid tenantId) =>
        await db.Entrenadores.FirstOrDefaultAsync(e => e.Id == tenantId)
            ?? throw new KeyNotFoundException("Entrenador no encontrado.");

    // ── Mapper ────────────────────────────────────────────────────────────────

    private static ConfiguracionDto ToDto(Entrenador e) => new()
    {
        Id            = e.Id,
        Nombre        = e.Nombre,
        Email         = e.Email,
        Plan          = e.Plan,
        TrialEndsAt   = e.TrialEndsAt,
        RedesSociales = e.RedesSociales == "{}"
            ? null
            : JsonSerializer.Deserialize<object>(e.RedesSociales),
        CreatedAt     = e.CreatedAt
    };
}

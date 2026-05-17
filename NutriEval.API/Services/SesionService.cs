using NutriEval.API.Models.DTOs.Sesiones;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class SesionService(
    ISesionRepository repository,
    IClienteRepository clienteRepository,
    ILogger<SesionService> logger) : ISesionService
{
    public async Task<IEnumerable<SesionDto>> GetAllByTenantAsync(Guid tenantId)
    {
        var sesiones = await repository.GetAllByTenantAsync(tenantId);
        return sesiones.Select(ToDto);
    }

    public async Task<IEnumerable<SesionDto>> GetAllByClienteAsync(Guid clienteId, Guid tenantId)
    {
        var cliente = await clienteRepository.GetByIdAsync(clienteId, tenantId)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var sesiones = await repository.GetAllByClienteAsync(clienteId, tenantId);
        return sesiones.Select(ToDto);
    }

    public async Task<SesionDto> CreateAsync(CreateSesionDto dto, Guid tenantId)
    {
        var cliente = await clienteRepository.GetByIdAsync(dto.ClienteId, tenantId)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var sesion = new Sesion
        {
            Id          = Guid.NewGuid(),
            TenantId    = tenantId,
            ClienteId   = dto.ClienteId,
            FechaHora   = dto.FechaHora,
            DuracionMin = dto.DuracionMin,
            Estado      = "programada",
            Notas       = dto.Notas?.Trim(),
            Cliente     = cliente
        };

        await repository.AddAsync(sesion);

        logger.LogInformation("Sesión creada: {Id} para cliente {ClienteId}", sesion.Id, dto.ClienteId);

        return ToDto(sesion);
    }

    public async Task<SesionDto> UpdateAsync(Guid id, UpdateSesionDto dto, Guid tenantId)
    {
        var sesion = await repository.GetByIdAsync(id, tenantId)
            ?? throw new KeyNotFoundException("Sesión no encontrada.");

        if (dto.FechaHora.HasValue)  sesion.FechaHora   = dto.FechaHora.Value;
        if (dto.DuracionMin.HasValue) sesion.DuracionMin = dto.DuracionMin.Value;
        if (dto.Estado is not null)  sesion.Estado      = dto.Estado;
        if (dto.Notas is not null)   sesion.Notas       = dto.Notas.Trim();

        await repository.SaveChangesAsync();

        logger.LogInformation("Sesión actualizada: {Id}", id);

        return ToDto(sesion);
    }

    public async Task DeleteAsync(Guid id, Guid tenantId)
    {
        var sesion = await repository.GetByIdAsync(id, tenantId)
            ?? throw new KeyNotFoundException("Sesión no encontrada.");

        sesion.Estado = "cancelada";
        await repository.SaveChangesAsync();

        logger.LogInformation("Sesión cancelada: {Id}", id);
    }

    // ── Mapper ────────────────────────────────────────────────────────────────

    private static SesionDto ToDto(Sesion s) => new()
    {
        Id            = s.Id,
        TenantId      = s.TenantId,
        ClienteId     = s.ClienteId,
        ClienteNombre = s.Cliente?.Nombre ?? string.Empty,
        FechaHora     = s.FechaHora,
        DuracionMin   = s.DuracionMin,
        Estado        = s.Estado,
        Notas         = s.Notas,
        CreatedAt     = s.CreatedAt
    };
}

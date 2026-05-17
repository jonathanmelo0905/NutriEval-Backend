using System.Text.Json;
using NutriEval.API.Models.DTOs.Evaluaciones;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class EvaluacionService(
    IEvaluacionRepository repository,
    IClienteRepository clienteRepository,
    ILogger<EvaluacionService> logger) : IEvaluacionService
{
    public async Task<IEnumerable<EvaluacionListItemDto>> GetAllAsync(Guid clienteId, Guid tenantId)
    {
        await VerificarClienteAsync(clienteId, tenantId);
        var evaluaciones = await repository.GetAllByClienteAsync(clienteId, tenantId);
        return evaluaciones.Select(ToListItem);
    }

    public async Task<EvaluacionDetalleDto> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId)
    {
        var evaluacion = await repository.GetByIdAsync(id, clienteId, tenantId)
            ?? throw new KeyNotFoundException("Evaluación no encontrada.");

        return ToDetalle(evaluacion);
    }

    public async Task<EvaluacionDetalleDto> CreateAsync(Guid clienteId, CreateEvaluacionDto dto, Guid tenantId)
    {
        await VerificarClienteAsync(clienteId, tenantId);

        var evaluacion = new Evaluacion
        {
            Id           = Guid.NewGuid(),
            TenantId     = tenantId,
            ClienteId    = clienteId,
            Tipo         = dto.Tipo,
            DatosEntrada = Serialize(dto.DatosEntrada),
            Resultados   = Serialize(dto.Resultados),
            Fecha        = dto.Fecha ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Notas        = dto.Notas?.Trim()
        };

        await repository.AddAsync(evaluacion);

        logger.LogInformation("Evaluación creada: {Id} para cliente {ClienteId}", evaluacion.Id, clienteId);

        return ToDetalle(evaluacion);
    }

    public async Task DeleteAsync(Guid id, Guid clienteId, Guid tenantId)
    {
        var evaluacion = await repository.GetByIdAsync(id, clienteId, tenantId)
            ?? throw new KeyNotFoundException("Evaluación no encontrada.");

        await repository.DeleteAsync(evaluacion);

        logger.LogInformation("Evaluación eliminada: {Id}", id);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task VerificarClienteAsync(Guid clienteId, Guid tenantId)
    {
        var existe = await clienteRepository.GetByIdAsync(clienteId, tenantId);
        if (existe is null)
            throw new KeyNotFoundException("Cliente no encontrado.");
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static EvaluacionListItemDto ToListItem(Evaluacion e) => new()
    {
        Id        = e.Id,
        ClienteId = e.ClienteId,
        Tipo      = e.Tipo,
        Fecha     = e.Fecha,
        Notas     = e.Notas,
        CreatedAt = e.CreatedAt
    };

    private static EvaluacionDetalleDto ToDetalle(Evaluacion e) => new()
    {
        Id           = e.Id,
        TenantId     = e.TenantId,
        ClienteId    = e.ClienteId,
        Tipo         = e.Tipo,
        DatosEntrada = ParseJsonb(e.DatosEntrada),
        Resultados   = ParseJsonb(e.Resultados),
        Fecha        = e.Fecha,
        Notas        = e.Notas,
        CreatedAt    = e.CreatedAt
    };

    // ── JSON helpers ─────────────────────────────────────────────────────────

    private static string Serialize(object? value) =>
        value is null ? "{}" : JsonSerializer.Serialize(value);

    private static object? ParseJsonb(string json) =>
        json == "{}" ? null : JsonSerializer.Deserialize<object>(json);
}

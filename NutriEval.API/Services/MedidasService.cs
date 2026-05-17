using NutriEval.API.Models.DTOs.Medidas;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class MedidasService(
    IMedidasRepository repository,
    IClienteRepository clienteRepository,
    ILogger<MedidasService> logger) : IMedidasService
{
    public async Task<IEnumerable<MedidaDto>> GetAllAsync(Guid clienteId, Guid tenantId)
    {
        await VerificarClienteAsync(clienteId, tenantId);
        var medidas = await repository.GetAllByClienteAsync(clienteId, tenantId);
        return medidas.Select(ToDto);
    }

    public async Task<MedidaDto> CreateAsync(Guid clienteId, CreateMedidaDto dto, Guid tenantId)
    {
        await VerificarClienteAsync(clienteId, tenantId);

        var medida = new MedidaCorporal
        {
            Id              = Guid.NewGuid(),
            TenantId        = tenantId,
            ClienteId       = clienteId,
            Peso            = dto.Peso,
            PorcentajeGrasa = dto.PorcentajeGrasa,
            MasaMuscular    = dto.MasaMuscular,
            Imc             = dto.Imc,
            Cintura         = dto.Cintura,
            Cadera          = dto.Cadera,
            Pecho           = dto.Pecho,
            BrazoDerecho    = dto.BrazoDerecho,
            BrazoIzquierdo  = dto.BrazoIzquierdo,
            PiernaDerecha   = dto.PiernaDerecha,
            PiernaIzquierda = dto.PiernaIzquierda,
            Fecha           = dto.Fecha ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Notas           = dto.Notas?.Trim()
        };

        await repository.AddAsync(medida);

        logger.LogInformation("Medida registrada: {Id} para cliente {ClienteId}", medida.Id, clienteId);

        return ToDto(medida);
    }

    public async Task DeleteAsync(Guid id, Guid clienteId, Guid tenantId)
    {
        var medida = await repository.GetByIdAsync(id, clienteId, tenantId)
            ?? throw new KeyNotFoundException("Registro de medida no encontrado.");

        await repository.DeleteAsync(medida);

        logger.LogInformation("Medida eliminada: {Id}", id);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task VerificarClienteAsync(Guid clienteId, Guid tenantId)
    {
        var existe = await clienteRepository.GetByIdAsync(clienteId, tenantId);
        if (existe is null)
            throw new KeyNotFoundException("Cliente no encontrado.");
    }

    // ── Mapper ────────────────────────────────────────────────────────────────

    private static MedidaDto ToDto(MedidaCorporal m) => new()
    {
        Id              = m.Id,
        TenantId        = m.TenantId,
        ClienteId       = m.ClienteId,
        Peso            = m.Peso,
        PorcentajeGrasa = m.PorcentajeGrasa,
        MasaMuscular    = m.MasaMuscular,
        Imc             = m.Imc,
        Cintura         = m.Cintura,
        Cadera          = m.Cadera,
        Pecho           = m.Pecho,
        BrazoDerecho    = m.BrazoDerecho,
        BrazoIzquierdo  = m.BrazoIzquierdo,
        PiernaDerecha   = m.PiernaDerecha,
        PiernaIzquierda = m.PiernaIzquierda,
        Fecha           = m.Fecha,
        Notas           = m.Notas,
        CreatedAt       = m.CreatedAt
    };
}

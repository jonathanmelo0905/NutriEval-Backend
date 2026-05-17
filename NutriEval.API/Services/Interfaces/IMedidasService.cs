using NutriEval.API.Models.DTOs.Medidas;

namespace NutriEval.API.Services.Interfaces;

public interface IMedidasService
{
    Task<IEnumerable<MedidaDto>> GetAllAsync(Guid clienteId, Guid tenantId);
    Task<MedidaDto> CreateAsync(Guid clienteId, CreateMedidaDto dto, Guid tenantId);
    Task DeleteAsync(Guid id, Guid clienteId, Guid tenantId);
}

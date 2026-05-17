using NutriEval.API.Models.DTOs.Evaluaciones;

namespace NutriEval.API.Services.Interfaces;

public interface IEvaluacionService
{
    Task<IEnumerable<EvaluacionListItemDto>> GetAllAsync(Guid clienteId, Guid tenantId);
    Task<EvaluacionDetalleDto> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId);
    Task<EvaluacionDetalleDto> CreateAsync(Guid clienteId, CreateEvaluacionDto dto, Guid tenantId);
    Task DeleteAsync(Guid id, Guid clienteId, Guid tenantId);
}

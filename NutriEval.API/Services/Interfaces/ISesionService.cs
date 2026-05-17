using NutriEval.API.Models.DTOs.Sesiones;

namespace NutriEval.API.Services.Interfaces;

public interface ISesionService
{
    Task<IEnumerable<SesionDto>> GetAllByTenantAsync(Guid tenantId);
    Task<IEnumerable<SesionDto>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<SesionDto> CreateAsync(CreateSesionDto dto, Guid tenantId);
    Task<SesionDto> UpdateAsync(Guid id, UpdateSesionDto dto, Guid tenantId);
    Task DeleteAsync(Guid id, Guid tenantId);
}

using NutriEval.API.Models.DTOs.Clientes;

namespace NutriEval.API.Services.Interfaces;

public interface IClienteService
{
    Task<IEnumerable<ClienteListItemDto>> GetAllAsync(Guid tenantId);
    Task<ClienteDetalleDto> GetByIdAsync(Guid id, Guid tenantId);
    Task<ClienteDetalleDto> CreateAsync(CreateClienteDto dto, Guid tenantId);
    Task<ClienteDetalleDto> UpdateAsync(Guid id, UpdateClienteDto dto, Guid tenantId);
    Task DeleteAsync(Guid id, Guid tenantId);
    Task InvitarAsync(Guid id, Guid tenantId);
}

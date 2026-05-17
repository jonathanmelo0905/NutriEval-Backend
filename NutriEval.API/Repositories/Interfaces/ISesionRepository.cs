using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface ISesionRepository
{
    Task<IEnumerable<Sesion>> GetAllByTenantAsync(Guid tenantId);
    Task<IEnumerable<Sesion>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<Sesion?> GetByIdAsync(Guid id, Guid tenantId);
    Task AddAsync(Sesion sesion);
    Task SaveChangesAsync();
}

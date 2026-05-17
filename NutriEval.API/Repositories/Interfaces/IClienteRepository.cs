using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> GetAllByTenantAsync(Guid tenantId);
    Task<Cliente?> GetByIdAsync(Guid id, Guid tenantId);
    Task<bool> ExisteEmailAsync(string email, Guid tenantId, Guid? excludeId = null);
    Task AddAsync(Cliente cliente);
    Task SaveChangesAsync();
}

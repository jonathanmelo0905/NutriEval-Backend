using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface IMedidasRepository
{
    Task<IEnumerable<MedidaCorporal>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<MedidaCorporal?> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId);
    Task AddAsync(MedidaCorporal medida);
    Task DeleteAsync(MedidaCorporal medida);
}

using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface IFotosRepository
{
    Task<IEnumerable<FotoProgreso>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<FotoProgreso?> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId);
    Task AddAsync(FotoProgreso foto);
    Task DeleteAsync(FotoProgreso foto);
}

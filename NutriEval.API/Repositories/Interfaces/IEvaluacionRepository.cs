using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface IEvaluacionRepository
{
    Task<IEnumerable<Evaluacion>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<Evaluacion?> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId);
    Task AddAsync(Evaluacion evaluacion);
    Task DeleteAsync(Evaluacion evaluacion);
}

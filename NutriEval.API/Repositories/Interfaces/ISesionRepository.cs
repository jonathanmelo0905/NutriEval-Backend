using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface ISesionRepository
{
    Task<IEnumerable<Sesion>> GetAllByTenantAsync(Guid tenantId);
    Task<IEnumerable<Sesion>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<Sesion?> GetByIdAsync(Guid id, Guid tenantId);
    /// <summary>
    /// Devuelve true si el tenant ya tiene una sesión con estado "programada"
    /// cuya fechaHora coincide exactamente con la indicada.
    /// </summary>
    Task<bool> ExistsSesionProgramadaAsync(Guid tenantId, DateTimeOffset fechaHora);
    Task AddAsync(Sesion sesion);
    Task SaveChangesAsync();
}

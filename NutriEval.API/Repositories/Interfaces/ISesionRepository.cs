using NutriEval.API.Models.Entities;

namespace NutriEval.API.Repositories.Interfaces;

public interface ISesionRepository
{
    Task<IEnumerable<Sesion>> GetAllByTenantAsync(Guid tenantId);
    Task<IEnumerable<Sesion>> GetAllByClienteAsync(Guid clienteId, Guid tenantId);
    Task<Sesion?> GetByIdAsync(Guid id, Guid tenantId);
    /// <summary>
    /// Devuelve true si el tenant ya tiene una sesión con estado "programada"
    /// cuyo intervalo [fechaHora, fechaHora+duracionMin) se solapa con
    /// el intervalo [inicio, inicio+duracionMin) de la sesión candidata.
    /// Condición estándar de solapamiento: existingStart &lt; candidatoFin AND candidatoInicio &lt; existingFin
    /// </summary>
    Task<bool> ExisteSolapamientoAsync(Guid tenantId, DateTimeOffset inicio, int duracionMin);
    Task AddAsync(Sesion sesion);
    Task SaveChangesAsync();
}

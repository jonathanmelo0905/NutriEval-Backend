using Microsoft.EntityFrameworkCore;
using NutriEval.API.Data;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;

namespace NutriEval.API.Repositories;

public class SesionRepository(NutriEvalDbContext db) : ISesionRepository
{
    public async Task<IEnumerable<Sesion>> GetAllByTenantAsync(Guid tenantId) =>
        await db.Sesiones
            .Include(s => s.Cliente)
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.FechaHora)
            .ToListAsync();

    public async Task<IEnumerable<Sesion>> GetAllByClienteAsync(Guid clienteId, Guid tenantId) =>
        await db.Sesiones
            .Include(s => s.Cliente)
            .Where(s => s.ClienteId == clienteId && s.TenantId == tenantId)
            .OrderByDescending(s => s.FechaHora)
            .ToListAsync();

    public async Task<Sesion?> GetByIdAsync(Guid id, Guid tenantId) =>
        await db.Sesiones
            .Include(s => s.Cliente)
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

    public async Task<bool> ExisteSolapamientoAsync(Guid tenantId, DateTimeOffset inicio, int duracionMin)
    {
        // Intervalo de la sesión candidata: [inicio, fin)
        var fin = inicio.AddMinutes(duracionMin);

        // Solapamiento estándar de intervalos:
        //   existingStart < candidatoFin  AND  candidatoInicio < existingFin
        // donde existingFin = s.FechaHora + s.DuracionMin minutos
        // Npgsql traduce .AddMinutes(int_column) a aritmética de intervalos en PostgreSQL.
        return await db.Sesiones.AnyAsync(s =>
            s.TenantId == tenantId        &&
            s.Estado   == "programada"    &&
            s.FechaHora         < fin     &&
            inicio < s.FechaHora.AddMinutes(s.DuracionMin));
    }

    public async Task AddAsync(Sesion sesion)
    {
        db.Sesiones.Add(sesion);
        await db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}

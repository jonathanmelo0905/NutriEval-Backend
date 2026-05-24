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

    public async Task<bool> ExistsSesionProgramadaAsync(Guid tenantId, DateTimeOffset fechaHora) =>
        await db.Sesiones.AnyAsync(s =>
            s.TenantId == tenantId &&
            s.Estado   == "programada" &&
            s.FechaHora == fechaHora);

    public async Task AddAsync(Sesion sesion)
    {
        db.Sesiones.Add(sesion);
        await db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}

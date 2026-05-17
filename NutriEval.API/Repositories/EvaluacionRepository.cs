using Microsoft.EntityFrameworkCore;
using NutriEval.API.Data;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;

namespace NutriEval.API.Repositories;

public class EvaluacionRepository(NutriEvalDbContext db) : IEvaluacionRepository
{
    public async Task<IEnumerable<Evaluacion>> GetAllByClienteAsync(Guid clienteId, Guid tenantId) =>
        await db.Evaluaciones
            .Where(e => e.ClienteId == clienteId && e.TenantId == tenantId)
            .OrderByDescending(e => e.Fecha)
            .ThenByDescending(e => e.CreatedAt)
            .ToListAsync();

    public async Task<Evaluacion?> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId) =>
        await db.Evaluaciones
            .FirstOrDefaultAsync(e => e.Id == id && e.ClienteId == clienteId && e.TenantId == tenantId);

    public async Task AddAsync(Evaluacion evaluacion)
    {
        db.Evaluaciones.Add(evaluacion);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Evaluacion evaluacion)
    {
        db.Evaluaciones.Remove(evaluacion);
        await db.SaveChangesAsync();
    }
}

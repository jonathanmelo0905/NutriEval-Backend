using Microsoft.EntityFrameworkCore;
using NutriEval.API.Data;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;

namespace NutriEval.API.Repositories;

public class FotosRepository(NutriEvalDbContext db) : IFotosRepository
{
    public async Task<IEnumerable<FotoProgreso>> GetAllByClienteAsync(Guid clienteId, Guid tenantId) =>
        await db.FotosProgreso
            .Where(f => f.ClienteId == clienteId && f.TenantId == tenantId)
            .OrderByDescending(f => f.Fecha)
            .ThenByDescending(f => f.CreatedAt)
            .ToListAsync();

    public async Task<FotoProgreso?> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId) =>
        await db.FotosProgreso
            .FirstOrDefaultAsync(f => f.Id == id && f.ClienteId == clienteId && f.TenantId == tenantId);

    public async Task AddAsync(FotoProgreso foto)
    {
        db.FotosProgreso.Add(foto);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(FotoProgreso foto)
    {
        db.FotosProgreso.Remove(foto);
        await db.SaveChangesAsync();
    }
}

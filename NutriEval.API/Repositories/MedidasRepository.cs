using Microsoft.EntityFrameworkCore;
using NutriEval.API.Data;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;

namespace NutriEval.API.Repositories;

public class MedidasRepository(NutriEvalDbContext db) : IMedidasRepository
{
    public async Task<IEnumerable<MedidaCorporal>> GetAllByClienteAsync(Guid clienteId, Guid tenantId) =>
        await db.MedidasCorporales
            .Where(m => m.ClienteId == clienteId && m.TenantId == tenantId)
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<MedidaCorporal?> GetByIdAsync(Guid id, Guid clienteId, Guid tenantId) =>
        await db.MedidasCorporales
            .FirstOrDefaultAsync(m => m.Id == id && m.ClienteId == clienteId && m.TenantId == tenantId);

    public async Task AddAsync(MedidaCorporal medida)
    {
        db.MedidasCorporales.Add(medida);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(MedidaCorporal medida)
    {
        db.MedidasCorporales.Remove(medida);
        await db.SaveChangesAsync();
    }
}

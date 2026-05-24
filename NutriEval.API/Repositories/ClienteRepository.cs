using Microsoft.EntityFrameworkCore;
using NutriEval.API.Data;
using NutriEval.API.Models.DTOs.Clientes;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;

namespace NutriEval.API.Repositories;

public class ClienteRepository(NutriEvalDbContext db) : IClienteRepository
{
    public async Task<IEnumerable<ClienteListItemDto>> GetAllByTenantAsync(Guid tenantId) =>
        await db.Clientes
            .Where(c => c.TenantId == tenantId && c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new ClienteListItemDto
            {
                Id                = c.Id,
                Nombre            = c.Nombre,
                Email             = c.Email,
                Objetivo          = c.Objetivo,
                Nivel             = c.Nivel,
                PesoInicial       = c.PesoInicial,
                Estatura          = c.Estatura,
                Activo            = c.Activo,
                CreatedAt         = c.CreatedAt,
                TieneFotos        = c.Fotos.Any(),
                TieneEvaluaciones = c.Evaluaciones.Any(),
                TieneSesiones     = c.Sesiones.Any(),
                TieneSalud        = c.Salud != null && c.Salud != "{}",
                TieneHabitos      = c.Habitos != null && c.Habitos != "{}"
            })
            .ToListAsync();

    public async Task<Cliente?> GetByIdAsync(Guid id, Guid tenantId) =>
        await db.Clientes
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

    public async Task<bool> ExisteEmailAsync(string email, Guid tenantId, Guid? excludeId = null) =>
        await db.Clientes.AnyAsync(c =>
            c.Email == email &&
            c.TenantId == tenantId &&
            (excludeId == null || c.Id != excludeId));

    public async Task AddAsync(Cliente cliente)
    {
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}

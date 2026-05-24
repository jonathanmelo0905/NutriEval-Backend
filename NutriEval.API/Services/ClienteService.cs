using System.Text.Json;
using BCrypt.Net;
using NutriEval.API.Models.DTOs.Clientes;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class ClienteService(
    IClienteRepository repository,
    ILogger<ClienteService> logger) : IClienteService
{
    public async Task<IEnumerable<ClienteListItemDto>> GetAllAsync(Guid tenantId) =>
        await repository.GetAllByTenantAsync(tenantId);

    public async Task<ClienteDetalleDto> GetByIdAsync(Guid id, Guid tenantId)
    {
        var cliente = await repository.GetByIdAsync(id, tenantId)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        return ToDetalle(cliente);
    }

    public async Task<ClienteDetalleDto> CreateAsync(CreateClienteDto dto, Guid tenantId)
    {
        var email = dto.Email?.ToLower().Trim();

        if (email is not null && await repository.ExisteEmailAsync(email, tenantId))
            throw new ArgumentException("Ya existe un cliente con ese email en tu cuenta.");

        var cliente = new Cliente
        {
            Id                 = Guid.NewGuid(),
            TenantId           = tenantId,
            Nombre             = dto.Nombre.Trim(),
            Email              = email,
            FechaNacimiento    = dto.FechaNacimiento,
            Sexo               = dto.Sexo,
            PesoInicial        = dto.PesoInicial,
            Estatura           = dto.Estatura,
            Objetivo           = dto.Objetivo,
            Nivel              = dto.Nivel,
            Telefono           = dto.Telefono?.Trim(),
            ContactoEmergencia = Serialize(dto.ContactoEmergencia),
            Salud              = Serialize(dto.Salud),
            Habitos            = Serialize(dto.Habitos),
            PasswordHash       = dto.PasswordTemporal is not null
                                     ? BCrypt.Net.BCrypt.HashPassword(dto.PasswordTemporal)
                                     : null
        };

        await repository.AddAsync(cliente);

        logger.LogInformation("Cliente creado: {Nombre} — tenant {TenantId}", cliente.Nombre, tenantId);

        return ToDetalle(cliente);
    }

    public async Task<ClienteDetalleDto> UpdateAsync(Guid id, UpdateClienteDto dto, Guid tenantId)
    {
        var cliente = await repository.GetByIdAsync(id, tenantId)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        var email = dto.Email?.ToLower().Trim();

        if (email is not null && email != cliente.Email)
        {
            if (await repository.ExisteEmailAsync(email, tenantId, excludeId: id))
                throw new ArgumentException("Ya existe un cliente con ese email en tu cuenta.");

            cliente.Email = email;
        }

        if (dto.Nombre is not null)            cliente.Nombre             = dto.Nombre.Trim();
        if (dto.FechaNacimiento.HasValue)       cliente.FechaNacimiento    = dto.FechaNacimiento;
        if (dto.Sexo is not null)              cliente.Sexo               = dto.Sexo;
        if (dto.PesoInicial.HasValue)           cliente.PesoInicial        = dto.PesoInicial;
        if (dto.Estatura.HasValue)              cliente.Estatura           = dto.Estatura;
        if (dto.Objetivo is not null)           cliente.Objetivo           = dto.Objetivo;
        if (dto.Nivel is not null)              cliente.Nivel              = dto.Nivel;
        if (dto.Telefono is not null)           cliente.Telefono           = dto.Telefono.Trim();
        if (dto.ContactoEmergencia is not null) cliente.ContactoEmergencia = Serialize(dto.ContactoEmergencia);
        if (dto.Salud is not null)             cliente.Salud              = Serialize(dto.Salud);
        if (dto.Habitos is not null)            cliente.Habitos            = Serialize(dto.Habitos);
        if (dto.Activo.HasValue)               cliente.Activo             = dto.Activo.Value;

        await repository.SaveChangesAsync();

        logger.LogInformation("Cliente actualizado: {Id}", id);

        return ToDetalle(cliente);
    }

    public async Task DeleteAsync(Guid id, Guid tenantId)
    {
        var cliente = await repository.GetByIdAsync(id, tenantId)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        cliente.Activo = false;
        await repository.SaveChangesAsync();

        logger.LogInformation("Cliente desactivado: {Id}", id);
    }

    public async Task InvitarAsync(Guid id, Guid tenantId)
    {
        var cliente = await repository.GetByIdAsync(id, tenantId)
            ?? throw new KeyNotFoundException("Cliente no encontrado.");

        if (string.IsNullOrEmpty(cliente.Email))
            throw new ArgumentException("El cliente no tiene email registrado. Actualízalo primero.");
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static ClienteDetalleDto ToDetalle(Cliente c) => new()
    {
        Id                     = c.Id,
        TenantId               = c.TenantId,
        Nombre                 = c.Nombre,
        Email                  = c.Email,
        FechaNacimiento        = c.FechaNacimiento,
        Sexo                   = c.Sexo,
        PesoInicial            = c.PesoInicial,
        Estatura               = c.Estatura,
        Objetivo               = c.Objetivo,
        Nivel                  = c.Nivel,
        Telefono               = c.Telefono,
        ContactoEmergencia     = ParseJsonb(c.ContactoEmergencia),
        Salud                  = ParseJsonb(c.Salud),
        Habitos                = ParseJsonb(c.Habitos),
        ParqCompletado         = c.ParqCompletado,
        ParqDatos              = ParseJsonb(c.ParqDatos),
        ParqFecha              = c.ParqFecha,
        ConsentimientoAceptado = c.ConsentimientoAceptado,
        ConsentimientoFecha    = c.ConsentimientoFecha,
        FotosIniciales         = ParseJsonb(c.FotosIniciales, emptyValue: "[]"),
        Activo                 = c.Activo,
        CreatedAt              = c.CreatedAt
    };

    // ── JSON helpers ─────────────────────────────────────────────────────────

    private static string Serialize(object? value, string fallback = "{}") =>
        value is null ? fallback : JsonSerializer.Serialize(value);

    private static object? ParseJsonb(string json, string emptyValue = "{}") =>
        json == emptyValue ? null : JsonSerializer.Deserialize<object>(json);
}

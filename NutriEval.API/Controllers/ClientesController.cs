using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Clientes;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[Authorize]
[Route("api/clientes")]
public class ClientesController(IClienteService clienteService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        RequireEntrenador();
        var clientes = await clienteService.GetAllAsync(TenantId);
        return Ok(ApiResponse.Ok(clientes));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto dto)
    {
        RequireEntrenador();
        var result = await clienteService.CreateAsync(dto, TenantId);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Cliente creado exitosamente."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        RequireEntrenador();
        var result = await clienteService.GetByIdAsync(id, TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClienteDto dto)
    {
        RequireEntrenador();
        var result = await clienteService.UpdateAsync(id, dto, TenantId);
        return Ok(ApiResponse.Ok(result, "Cliente actualizado."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        RequireEntrenador();
        await clienteService.DeleteAsync(id, TenantId);
        return Ok(ApiResponse.Ok<object?>(null, "Cliente desactivado."));
    }

    [HttpPost("{id:guid}/invitar")]
    public async Task<IActionResult> Invitar(Guid id)
    {
        RequireEntrenador();
        await clienteService.InvitarAsync(id, TenantId);
        return Accepted(ApiResponse.Ok<object?>(null,
            "Invitación pendiente. El sistema de emails estará disponible en v2.0."));
    }
}

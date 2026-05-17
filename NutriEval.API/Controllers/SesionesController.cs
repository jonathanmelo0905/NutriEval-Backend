using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Sesiones;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[Authorize]
[Route("api/sesiones")]
public class SesionesController(ISesionService sesionService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        RequireEntrenador();
        var result = await sesionService.GetAllByTenantAsync(TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSesionDto dto)
    {
        RequireEntrenador();
        var result = await sesionService.CreateAsync(dto, TenantId);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Sesión creada exitosamente."));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSesionDto dto)
    {
        RequireEntrenador();
        var result = await sesionService.UpdateAsync(id, dto, TenantId);
        return Ok(ApiResponse.Ok(result, "Sesión actualizada."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        RequireEntrenador();
        await sesionService.DeleteAsync(id, TenantId);
        return Ok(ApiResponse.Ok<object?>(null, "Sesión cancelada."));
    }

    // Ruta anidada bajo cliente
    [HttpGet("~/api/clientes/{clienteId:guid}/sesiones")]
    public async Task<IActionResult> GetByCliente(Guid clienteId)
    {
        RequireEntrenador();
        var result = await sesionService.GetAllByClienteAsync(clienteId, TenantId);
        return Ok(ApiResponse.Ok(result));
    }
}

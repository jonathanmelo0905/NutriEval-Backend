using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Medidas;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[Authorize]
[Route("api/clientes/{clienteId:guid}/medidas")]
public class MedidasController(IMedidasService medidasService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clienteId)
    {
        RequireEntrenador();
        var result = await medidasService.GetAllAsync(clienteId, TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid clienteId, [FromBody] CreateMedidaDto dto)
    {
        RequireEntrenador();
        var result = await medidasService.CreateAsync(clienteId, dto, TenantId);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Medida registrada exitosamente."));
    }

    [HttpDelete("{mid:guid}")]
    public async Task<IActionResult> Delete(Guid clienteId, Guid mid)
    {
        RequireEntrenador();
        await medidasService.DeleteAsync(mid, clienteId, TenantId);
        return Ok(ApiResponse.Ok<object?>(null, "Medida eliminada."));
    }
}

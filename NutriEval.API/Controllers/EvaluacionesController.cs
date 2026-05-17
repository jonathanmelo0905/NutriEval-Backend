using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Evaluaciones;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[Authorize]
[Route("api/clientes/{clienteId:guid}/evaluaciones")]
public class EvaluacionesController(IEvaluacionService evaluacionService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clienteId)
    {
        RequireEntrenador();
        var result = await evaluacionService.GetAllAsync(clienteId, TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid clienteId, [FromBody] CreateEvaluacionDto dto)
    {
        RequireEntrenador();
        var result = await evaluacionService.CreateAsync(clienteId, dto, TenantId);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Evaluación creada exitosamente."));
    }

    [HttpGet("{evalId:guid}")]
    public async Task<IActionResult> GetById(Guid clienteId, Guid evalId)
    {
        RequireEntrenador();
        var result = await evaluacionService.GetByIdAsync(evalId, clienteId, TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpDelete("{evalId:guid}")]
    public async Task<IActionResult> Delete(Guid clienteId, Guid evalId)
    {
        RequireEntrenador();
        await evaluacionService.DeleteAsync(evalId, clienteId, TenantId);
        return Ok(ApiResponse.Ok<object?>(null, "Evaluación eliminada."));
    }
}

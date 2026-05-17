using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Fotos;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[Authorize]
[Route("api/clientes/{clienteId:guid}/fotos")]
public class FotosController(IFotosService fotosService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(Guid clienteId)
    {
        RequireEntrenador();
        var result = await fotosService.GetAllAsync(clienteId, TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Subir(Guid clienteId, [FromForm] UploadFotoDto dto)
    {
        RequireEntrenador();
        var result = await fotosService.SubirAsync(clienteId, dto, TenantId);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Foto subida exitosamente."));
    }

    [HttpDelete("{fid:guid}")]
    public async Task<IActionResult> Eliminar(Guid clienteId, Guid fid)
    {
        RequireEntrenador();
        await fotosService.EliminarAsync(fid, clienteId, TenantId);
        return Ok(ApiResponse.Ok<object?>(null, "Foto eliminada."));
    }
}

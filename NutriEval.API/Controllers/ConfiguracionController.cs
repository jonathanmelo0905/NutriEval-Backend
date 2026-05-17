using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Configuracion;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[Authorize]
[Route("api/configuracion")]
public class ConfiguracionController(IConfiguracionService configuracionService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        RequireEntrenador();
        var result = await configuracionService.GetAsync(TenantId);
        return Ok(ApiResponse.Ok(result));
    }

    [HttpPut("redes")]
    public async Task<IActionResult> UpdateRedes([FromBody] UpdateRedesDto dto)
    {
        RequireEntrenador();
        var result = await configuracionService.UpdateRedesAsync(TenantId, dto);
        return Ok(ApiResponse.Ok(result, "Redes sociales actualizadas."));
    }
}

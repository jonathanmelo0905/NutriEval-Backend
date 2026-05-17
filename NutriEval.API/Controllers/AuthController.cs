using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriEval.API.Models;
using NutriEval.API.Models.DTOs.Auth;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register-entrenador")]
    public async Task<IActionResult> RegisterEntrenador([FromBody] RegisterEntrenadorDto dto)
    {
        var result = await authService.RegisterEntrenadorAsync(dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse.Ok(result, "Entrenador registrado exitosamente."));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return Ok(ApiResponse.Ok(result, "Login exitoso."));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await authService.RefreshAsync(dto);
        return Ok(ApiResponse.Ok(result, "Token renovado."));
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() =>
        Ok(ApiResponse.Ok<object?>(null, "Sesión cerrada. Elimina el token en el cliente."));

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userIdStr = User.FindFirstValue("user_id");
        var tipo      = User.FindFirstValue("tipo");

        if (userIdStr is null || tipo is null || !Guid.TryParse(userIdStr, out var userId))
            return Unauthorized(ApiResponse.Fail("Token inválido."));

        var me = await authService.GetMeAsync(userId, tipo);
        return Ok(ApiResponse.Ok(me));
    }
}

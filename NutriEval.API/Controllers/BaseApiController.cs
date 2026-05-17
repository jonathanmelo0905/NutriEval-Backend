using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace NutriEval.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected Guid TenantId =>
        HttpContext.Items.TryGetValue("TenantId", out var v) && v is Guid id
            ? id
            : throw new UnauthorizedAccessException("No se pudo determinar el tenant.");

    protected string TipoUsuario =>
        User.FindFirstValue("tipo")
            ?? throw new UnauthorizedAccessException("Tipo de usuario no determinado.");

    protected void RequireEntrenador()
    {
        if (TipoUsuario != "entrenador")
            throw new UnauthorizedAccessException("Solo los entrenadores pueden acceder a este recurso.");
    }
}

namespace NutriEval.API.Models.Entities;

public class Sesion
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public DateTimeOffset FechaHora { get; set; }
    public int DuracionMin { get; set; } = 60;
    public string Estado { get; set; } = "programada";
    public string? Notas { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

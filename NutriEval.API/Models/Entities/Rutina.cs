namespace NutriEval.API.Models.Entities;

public class Rutina
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Dias { get; set; } = "[]";
    public bool Activa { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

namespace NutriEval.API.Models.Entities;

public class Ejercicio
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? GrupoMuscular { get; set; }
    public string? Instrucciones { get; set; }
    public string? UrlVideo { get; set; }
    public string? ErroresComunes { get; set; }
    public bool EsGlobal { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

namespace NutriEval.API.Models.Entities;

public class Evaluacion
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string DatosEntrada { get; set; } = "{}";
    public string Resultados { get; set; } = "{}";
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Notas { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

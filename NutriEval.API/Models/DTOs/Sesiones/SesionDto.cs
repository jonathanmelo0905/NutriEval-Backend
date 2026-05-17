namespace NutriEval.API.Models.DTOs.Sesiones;

public class SesionDto
{
    public Guid            Id           { get; init; }
    public Guid            TenantId     { get; init; }
    public Guid            ClienteId    { get; init; }
    public string          ClienteNombre { get; init; } = string.Empty;
    public DateTimeOffset  FechaHora    { get; init; }
    public int             DuracionMin  { get; init; }
    public string          Estado       { get; init; } = string.Empty;
    public string?         Notas        { get; init; }
    public DateTimeOffset  CreatedAt    { get; init; }
}

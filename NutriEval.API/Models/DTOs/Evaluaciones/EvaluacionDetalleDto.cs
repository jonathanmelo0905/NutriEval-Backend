namespace NutriEval.API.Models.DTOs.Evaluaciones;

public class EvaluacionDetalleDto
{
    public Guid       Id           { get; init; }
    public Guid       TenantId     { get; init; }
    public Guid       ClienteId    { get; init; }
    public string     Tipo         { get; init; } = string.Empty;
    public object?    DatosEntrada { get; init; }
    public object?    Resultados   { get; init; }
    public DateOnly   Fecha        { get; init; }
    public string?    Notas        { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

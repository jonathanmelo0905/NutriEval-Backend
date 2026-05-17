namespace NutriEval.API.Models.DTOs.Evaluaciones;

public class EvaluacionListItemDto
{
    public Guid       Id        { get; init; }
    public Guid       ClienteId { get; init; }
    public string     Tipo      { get; init; } = string.Empty;
    public DateOnly   Fecha     { get; init; }
    public string?    Notas     { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

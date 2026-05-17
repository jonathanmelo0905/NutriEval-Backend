namespace NutriEval.API.Models.DTOs.Configuracion;

public class ConfiguracionDto
{
    public Guid       Id            { get; init; }
    public string     Nombre        { get; init; } = string.Empty;
    public string     Email         { get; init; } = string.Empty;
    public string     Plan          { get; init; } = string.Empty;
    public DateOnly?  TrialEndsAt   { get; init; }
    public object?    RedesSociales { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

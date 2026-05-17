namespace NutriEval.API.Models.DTOs.Fotos;

public class FotoDto
{
    public Guid       Id             { get; init; }
    public Guid       TenantId       { get; init; }
    public Guid       ClienteId      { get; init; }
    public string     UrlCloudinary  { get; init; } = string.Empty;
    public string     PublicId       { get; init; } = string.Empty;
    public string     Tipo           { get; init; } = string.Empty;
    public DateOnly   Fecha          { get; init; }
    public string?    MesReferencia  { get; init; }
    public string     SubidoPor      { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt  { get; init; }
}

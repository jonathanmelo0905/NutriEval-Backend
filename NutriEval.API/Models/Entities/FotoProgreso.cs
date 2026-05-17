namespace NutriEval.API.Models.Entities;

public class FotoProgreso
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public string UrlCloudinary { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? MesReferencia { get; set; }
    public string SubidoPor { get; set; } = "entrenador";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

namespace NutriEval.API.Models.Entities;

public class Checkin
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public DateOnly Semana { get; set; }
    public int? Energia { get; set; }
    public int? Hambre { get; set; }
    public int? Adherencia { get; set; }
    public int? Sueno { get; set; }
    public int? Estres { get; set; }
    public decimal? PesoSemana { get; set; }
    public string? NotasCliente { get; set; }
    public string? FeedbackEntrenador { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

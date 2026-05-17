namespace NutriEval.API.Models.Entities;

public class MedidaCorporal
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public decimal? Peso { get; set; }
    public decimal? PorcentajeGrasa { get; set; }
    public decimal? MasaMuscular { get; set; }
    public decimal? Imc { get; set; }
    public decimal? Cintura { get; set; }
    public decimal? Cadera { get; set; }
    public decimal? Pecho { get; set; }
    public decimal? BrazoDerecho { get; set; }
    public decimal? BrazoIzquierdo { get; set; }
    public decimal? PiernaDerecha { get; set; }
    public decimal? PiernaIzquierda { get; set; }
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Notas { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

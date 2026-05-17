namespace NutriEval.API.Models.Entities;

public class Pago
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = "COP";
    public string Estado { get; set; } = "pendiente";
    public string? Proveedor { get; set; }
    public string? ReferenciaExterna { get; set; }
    public DateOnly? VenceEn { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

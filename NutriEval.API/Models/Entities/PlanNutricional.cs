namespace NutriEval.API.Models.Entities;

public class PlanNutricional
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ClienteId { get; set; }
    public decimal? Calorias { get; set; }
    public decimal? Proteinas { get; set; }
    public decimal? Carbohidratos { get; set; }
    public decimal? Grasas { get; set; }
    public decimal? Agua { get; set; }
    public string Comidas { get; set; } = "[]";
    public bool Activo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Cliente Cliente { get; set; } = null!;
}

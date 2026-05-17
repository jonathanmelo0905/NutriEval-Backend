namespace NutriEval.API.Models.Entities;

public class SuscripcionSaas
{
    public Guid Id { get; set; }
    public Guid EntrenadorId { get; set; }
    public string Plan { get; set; } = string.Empty;
    public string Estado { get; set; } = "activa";
    public DateOnly? TrialEndsAt { get; set; }
    public DateOnly? NextBilling { get; set; }
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Entrenador Entrenador { get; set; } = null!;
}

namespace NutriEval.API.Models.Entities;

public class Entrenador
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Plan { get; set; } = "trial";
    public DateOnly? TrialEndsAt { get; set; }
    public string RedesSociales { get; set; } = "{}";
    public bool Activo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Cliente> Clientes { get; set; } = [];
    public ICollection<SuscripcionSaas> Suscripciones { get; set; } = [];
}

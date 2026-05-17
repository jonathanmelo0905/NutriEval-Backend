namespace NutriEval.API.Models.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public decimal? PesoInicial { get; set; }
    public decimal? Estatura { get; set; }
    public string? Objetivo { get; set; }
    public string? Nivel { get; set; }
    public string? Telefono { get; set; }
    public string ContactoEmergencia { get; set; } = "{}";
    public string Salud { get; set; } = "{}";
    public string Habitos { get; set; } = "{}";
    public bool ParqCompletado { get; set; } = false;
    public string ParqDatos { get; set; } = "{}";
    public DateTimeOffset? ParqFecha { get; set; }
    public bool ConsentimientoAceptado { get; set; } = false;
    public DateTimeOffset? ConsentimientoFecha { get; set; }
    public string FotosIniciales { get; set; } = "[]";
    public bool Activo { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Entrenador Entrenador { get; set; } = null!;
    public ICollection<Evaluacion> Evaluaciones { get; set; } = [];
    public ICollection<MedidaCorporal> Medidas { get; set; } = [];
    public ICollection<FotoProgreso> Fotos { get; set; } = [];
    public ICollection<Sesion> Sesiones { get; set; } = [];
    public ICollection<Rutina> Rutinas { get; set; } = [];
    public ICollection<PlanNutricional> Planes { get; set; } = [];
    public ICollection<Checkin> Checkins { get; set; } = [];
    public ICollection<Pago> Pagos { get; set; } = [];
}

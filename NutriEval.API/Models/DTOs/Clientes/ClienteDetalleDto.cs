namespace NutriEval.API.Models.DTOs.Clientes;

public class ClienteDetalleDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public string? Sexo { get; set; }
    public decimal? PesoInicial { get; set; }
    public decimal? Estatura { get; set; }
    public string? Objetivo { get; set; }
    public string? Nivel { get; set; }
    public string? Telefono { get; set; }
    public object? ContactoEmergencia { get; set; }
    public object? Salud { get; set; }
    public object? Habitos { get; set; }
    public bool ParqCompletado { get; set; }
    public object? ParqDatos { get; set; }
    public DateTimeOffset? ParqFecha { get; set; }
    public bool ConsentimientoAceptado { get; set; }
    public DateTimeOffset? ConsentimientoFecha { get; set; }
    public object? FotosIniciales { get; set; }
    public bool Activo { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

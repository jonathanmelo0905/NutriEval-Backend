namespace NutriEval.API.Models.DTOs.Clientes;

public class ClienteListItemDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Objetivo { get; set; }
    public string? Nivel { get; set; }
    public decimal? PesoInicial { get; set; }
    public bool Activo { get; set; }
    public decimal? Estatura { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

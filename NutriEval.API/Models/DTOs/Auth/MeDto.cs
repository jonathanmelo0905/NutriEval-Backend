namespace NutriEval.API.Models.DTOs.Auth;

public class MeDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Plan { get; set; }
}

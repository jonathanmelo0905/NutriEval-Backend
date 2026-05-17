using FluentValidation;

namespace NutriEval.API.Models.DTOs.Sesiones;

public class UpdateSesionDto
{
    public DateTimeOffset? FechaHora   { get; init; }
    public int?            DuracionMin { get; init; }
    public string?         Estado      { get; init; }
    public string?         Notas       { get; init; }
}

public class UpdateSesionValidator : AbstractValidator<UpdateSesionDto>
{
    private static readonly string[] EstadosValidos =
        ["programada", "completada", "cancelada", "no_asistio"];

    public UpdateSesionValidator()
    {
        RuleFor(x => x.DuracionMin)
            .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos.")
            .LessThanOrEqualTo(480).WithMessage("La duración no puede superar las 8 horas (480 min).")
            .When(x => x.DuracionMin.HasValue);

        RuleFor(x => x.Estado)
            .Must(e => EstadosValidos.Contains(e))
            .WithMessage("El estado debe ser 'programada', 'completada', 'cancelada' o 'no_asistio'.")
            .When(x => x.Estado is not null);

        RuleFor(x => x.Notas)
            .MaximumLength(2000).WithMessage("Las notas no pueden superar los 2000 caracteres.")
            .When(x => x.Notas is not null);
    }
}

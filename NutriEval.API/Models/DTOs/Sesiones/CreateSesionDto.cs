using FluentValidation;

namespace NutriEval.API.Models.DTOs.Sesiones;

public class CreateSesionDto
{
    public Guid            ClienteId   { get; init; }
    public DateTimeOffset  FechaHora   { get; init; }
    public int             DuracionMin { get; init; } = 60;
    public string?         Notas       { get; init; }
}

public class CreateSesionValidator : AbstractValidator<CreateSesionDto>
{
    public CreateSesionValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("El cliente es obligatorio.");

        RuleFor(x => x.FechaHora)
            .NotEmpty().WithMessage("La fecha y hora son obligatorias.");

        RuleFor(x => x.DuracionMin)
            .GreaterThan(0).WithMessage("La duración debe ser mayor a 0 minutos.")
            .LessThanOrEqualTo(480).WithMessage("La duración no puede superar las 8 horas (480 min).");

        RuleFor(x => x.Notas)
            .MaximumLength(2000).WithMessage("Las notas no pueden superar los 2000 caracteres.")
            .When(x => x.Notas is not null);
    }
}

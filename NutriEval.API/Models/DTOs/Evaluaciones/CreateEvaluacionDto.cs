using FluentValidation;

namespace NutriEval.API.Models.DTOs.Evaluaciones;

public class CreateEvaluacionDto
{
    public string   Tipo         { get; init; } = string.Empty;
    public object?  DatosEntrada { get; init; }
    public object?  Resultados   { get; init; }
    public DateOnly? Fecha       { get; init; }
    public string?  Notas        { get; init; }
}

public class CreateEvaluacionValidator : AbstractValidator<CreateEvaluacionDto>
{
    private static readonly string[] TiposValidos = ["nutricional", "fisica"];

    public CreateEvaluacionValidator()
    {
        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El tipo de evaluación es obligatorio.")
            .Must(t => TiposValidos.Contains(t))
            .WithMessage("El tipo debe ser 'nutricional' o 'fisica'.");

        RuleFor(x => x.DatosEntrada)
            .NotNull().WithMessage("Los datos de entrada son obligatorios.");

        RuleFor(x => x.Resultados)
            .NotNull().WithMessage("Los resultados son obligatorios.");

        RuleFor(x => x.Notas)
            .MaximumLength(2000).WithMessage("Las notas no pueden superar los 2000 caracteres.")
            .When(x => x.Notas is not null);
    }
}

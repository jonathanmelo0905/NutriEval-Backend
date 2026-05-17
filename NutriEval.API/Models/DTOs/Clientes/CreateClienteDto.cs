using FluentValidation;

namespace NutriEval.API.Models.DTOs.Clientes;

public class CreateClienteDto
{
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
}

public class CreateClienteValidator : AbstractValidator<CreateClienteDto>
{
    private static readonly string[] ObjetivosValidos =
        ["bajar_grasa", "ganar_musculo", "recomposicion", "rendimiento"];
    private static readonly string[] NivelesValidos =
        ["principiante", "intermedio", "avanzado"];
    private static readonly string[] SexosValidos =
        ["masculino", "femenino"];

    public CreateClienteValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El email no tiene un formato válido.")
            .MaximumLength(150).WithMessage("El email no puede exceder 150 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Sexo)
            .Must(s => SexosValidos.Contains(s))
            .WithMessage($"Sexo debe ser: {string.Join(", ", SexosValidos)}.")
            .When(x => x.Sexo is not null);

        RuleFor(x => x.Objetivo)
            .Must(o => ObjetivosValidos.Contains(o))
            .WithMessage($"Objetivo debe ser: {string.Join(", ", ObjetivosValidos)}.")
            .When(x => x.Objetivo is not null);

        RuleFor(x => x.Nivel)
            .Must(n => NivelesValidos.Contains(n))
            .WithMessage($"Nivel debe ser: {string.Join(", ", NivelesValidos)}.")
            .When(x => x.Nivel is not null);

        RuleFor(x => x.PesoInicial)
            .GreaterThan(0).WithMessage("El peso debe ser mayor a 0.")
            .When(x => x.PesoInicial.HasValue);

        RuleFor(x => x.Estatura)
            .GreaterThan(0).WithMessage("La estatura debe ser mayor a 0.")
            .When(x => x.Estatura.HasValue);
    }
}

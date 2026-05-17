using FluentValidation;

namespace NutriEval.API.Models.DTOs.Medidas;

public class CreateMedidaDto
{
    public decimal?  Peso            { get; init; }
    public decimal?  PorcentajeGrasa { get; init; }
    public decimal?  MasaMuscular    { get; init; }
    public decimal?  Imc             { get; init; }
    public decimal?  Cintura         { get; init; }
    public decimal?  Cadera          { get; init; }
    public decimal?  Pecho           { get; init; }
    public decimal?  BrazoDerecho    { get; init; }
    public decimal?  BrazoIzquierdo  { get; init; }
    public decimal?  PiernaDerecha   { get; init; }
    public decimal?  PiernaIzquierda { get; init; }
    public DateOnly? Fecha           { get; init; }
    public string?   Notas           { get; init; }
}

public class CreateMedidaValidator : AbstractValidator<CreateMedidaDto>
{
    public CreateMedidaValidator()
    {
        RuleFor(x => x).Must(TieneAlMenosUnaMedida)
            .WithMessage("Debe proporcionar al menos una medida.");

        RuleFor(x => x.Peso).GreaterThan(0).When(x => x.Peso.HasValue)
            .WithMessage("El peso debe ser mayor a 0.");
        RuleFor(x => x.PorcentajeGrasa).InclusiveBetween(0, 100).When(x => x.PorcentajeGrasa.HasValue)
            .WithMessage("El porcentaje de grasa debe estar entre 0 y 100.");
        RuleFor(x => x.MasaMuscular).GreaterThan(0).When(x => x.MasaMuscular.HasValue)
            .WithMessage("La masa muscular debe ser mayor a 0.");
        RuleFor(x => x.Imc).GreaterThan(0).When(x => x.Imc.HasValue)
            .WithMessage("El IMC debe ser mayor a 0.");
        RuleFor(x => x.Cintura).GreaterThan(0).When(x => x.Cintura.HasValue)
            .WithMessage("La cintura debe ser mayor a 0.");
        RuleFor(x => x.Cadera).GreaterThan(0).When(x => x.Cadera.HasValue)
            .WithMessage("La cadera debe ser mayor a 0.");
        RuleFor(x => x.Pecho).GreaterThan(0).When(x => x.Pecho.HasValue)
            .WithMessage("El pecho debe ser mayor a 0.");
        RuleFor(x => x.BrazoDerecho).GreaterThan(0).When(x => x.BrazoDerecho.HasValue)
            .WithMessage("El brazo derecho debe ser mayor a 0.");
        RuleFor(x => x.BrazoIzquierdo).GreaterThan(0).When(x => x.BrazoIzquierdo.HasValue)
            .WithMessage("El brazo izquierdo debe ser mayor a 0.");
        RuleFor(x => x.PiernaDerecha).GreaterThan(0).When(x => x.PiernaDerecha.HasValue)
            .WithMessage("La pierna derecha debe ser mayor a 0.");
        RuleFor(x => x.PiernaIzquierda).GreaterThan(0).When(x => x.PiernaIzquierda.HasValue)
            .WithMessage("La pierna izquierda debe ser mayor a 0.");
        RuleFor(x => x.Notas).MaximumLength(2000).When(x => x.Notas is not null)
            .WithMessage("Las notas no pueden superar los 2000 caracteres.");
    }

    private static bool TieneAlMenosUnaMedida(CreateMedidaDto dto) =>
        dto.Peso.HasValue || dto.PorcentajeGrasa.HasValue || dto.MasaMuscular.HasValue ||
        dto.Imc.HasValue  || dto.Cintura.HasValue          || dto.Cadera.HasValue       ||
        dto.Pecho.HasValue || dto.BrazoDerecho.HasValue    || dto.BrazoIzquierdo.HasValue ||
        dto.PiernaDerecha.HasValue || dto.PiernaIzquierda.HasValue;
}

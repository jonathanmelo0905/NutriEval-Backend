using FluentValidation;
using System.Text.RegularExpressions;

namespace NutriEval.API.Models.DTOs.Fotos;

public class UploadFotoDto
{
    public IFormFile? Archivo       { get; init; }
    public string     Tipo          { get; init; } = string.Empty;
    public DateOnly?  Fecha         { get; init; }
    public string?    MesReferencia { get; init; }
}

public class UploadFotoValidator : AbstractValidator<UploadFotoDto>
{
    private static readonly string[] TiposValidos = ["frontal", "lateral_der", "lateral_izq", "espalda"];
    private static readonly string[] ExtensionesValidas = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxBytes = 10 * 1024 * 1024; // 10 MB

    public UploadFotoValidator()
    {
        RuleFor(x => x.Archivo)
            .NotNull().WithMessage("El archivo es obligatorio.")
            .Must(f => f!.Length > 0).WithMessage("El archivo no puede estar vacío.")
            .Must(f => f!.Length <= MaxBytes).WithMessage("El archivo no puede superar los 10 MB.")
            .Must(f => ExtensionesValidas.Contains(
                Path.GetExtension(f!.FileName).ToLowerInvariant()))
            .WithMessage("Solo se permiten imágenes JPG, JPEG, PNG o WEBP.")
            .When(x => x.Archivo is not null);

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El tipo de foto es obligatorio.")
            .Must(t => TiposValidos.Contains(t))
            .WithMessage("El tipo debe ser 'frontal', 'lateral_der', 'lateral_izq' o 'espalda'.");

        RuleFor(x => x.MesReferencia)
            .Matches(@"^\d{4}-(0[1-9]|1[0-2])$")
            .WithMessage("El mes de referencia debe tener el formato YYYY-MM.")
            .When(x => x.MesReferencia is not null);
    }
}

using FluentValidation;

namespace NutriEval.API.Models.DTOs.Configuracion;

public class UpdateRedesDto
{
    public string? Instagram { get; init; }
    public string? Facebook  { get; init; }
    public string? Tiktok    { get; init; }
    public string? Web       { get; init; }
}

public class UpdateRedesValidator : AbstractValidator<UpdateRedesDto>
{
    public UpdateRedesValidator()
    {
        RuleFor(x => x.Instagram)
            .MaximumLength(100).WithMessage("Instagram no puede superar los 100 caracteres.")
            .When(x => x.Instagram is not null);

        RuleFor(x => x.Facebook)
            .MaximumLength(100).WithMessage("Facebook no puede superar los 100 caracteres.")
            .When(x => x.Facebook is not null);

        RuleFor(x => x.Tiktok)
            .MaximumLength(100).WithMessage("TikTok no puede superar los 100 caracteres.")
            .When(x => x.Tiktok is not null);

        RuleFor(x => x.Web)
            .MaximumLength(200).WithMessage("La URL web no puede superar los 200 caracteres.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("La URL web no es válida.")
            .When(x => !string.IsNullOrEmpty(x.Web));
    }
}

namespace NutriEval.API.Exceptions;

/// <summary>
/// Excepción que representa un conflicto de negocio (HTTP 409).
/// Lleva un array de códigos de error legibles por máquina.
/// </summary>
public class ConflictException(string message, IEnumerable<string>? codes = null)
    : Exception(message)
{
    public IEnumerable<string> Codes { get; } = codes ?? [];
}

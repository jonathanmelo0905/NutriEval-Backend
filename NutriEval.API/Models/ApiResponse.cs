namespace NutriEval.API.Models;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = string.Empty;
    public IEnumerable<string> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T data, string message = "Operación exitosa") =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Data = default, Message = message, Errors = errors ?? [] };
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data, string message = "Operación exitosa") =>
        ApiResponse<T>.Ok(data, message);

    public static ApiResponse<object?> Fail(string message, IEnumerable<string>? errors = null) =>
        ApiResponse<object?>.Fail(message, errors);
}

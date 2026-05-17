namespace NutriEval.API.Services.Interfaces;

public interface ICloudinaryService
{
    Task<(string UrlCloudinary, string PublicId)> SubirAsync(IFormFile archivo, string folder);
    Task EliminarAsync(string publicId);
}

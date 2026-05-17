using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class CloudinaryService(Cloudinary cloudinary, ILogger<CloudinaryService> logger) : ICloudinaryService
{
    public async Task<(string UrlCloudinary, string PublicId)> SubirAsync(IFormFile archivo, string folder)
    {
        await using var stream = archivo.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File        = new FileDescription(archivo.FileName, stream),
            Folder      = folder,
            UseFilename = false,
            Overwrite   = false
        };

        var result = await cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            logger.LogError("Error Cloudinary al subir: {Error}", result.Error.Message);
            throw new InvalidOperationException($"Error al subir la imagen: {result.Error.Message}");
        }

        logger.LogInformation("Foto subida a Cloudinary: {PublicId}", result.PublicId);

        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task EliminarAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null)
            logger.LogWarning("Error Cloudinary al eliminar {PublicId}: {Error}", publicId, result.Error.Message);
        else
            logger.LogInformation("Foto eliminada de Cloudinary: {PublicId}", publicId);
    }
}

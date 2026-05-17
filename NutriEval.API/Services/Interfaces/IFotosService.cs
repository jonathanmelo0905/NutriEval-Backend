using NutriEval.API.Models.DTOs.Fotos;

namespace NutriEval.API.Services.Interfaces;

public interface IFotosService
{
    Task<IEnumerable<FotoDto>> GetAllAsync(Guid clienteId, Guid tenantId);
    Task<FotoDto> SubirAsync(Guid clienteId, UploadFotoDto dto, Guid tenantId);
    Task EliminarAsync(Guid id, Guid clienteId, Guid tenantId);
}

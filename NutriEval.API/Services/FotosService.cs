using NutriEval.API.Models.DTOs.Fotos;
using NutriEval.API.Models.Entities;
using NutriEval.API.Repositories.Interfaces;
using NutriEval.API.Services.Interfaces;

namespace NutriEval.API.Services;

public class FotosService(
    IFotosRepository repository,
    IClienteRepository clienteRepository,
    ICloudinaryService cloudinaryService,
    ILogger<FotosService> logger) : IFotosService
{
    public async Task<IEnumerable<FotoDto>> GetAllAsync(Guid clienteId, Guid tenantId)
    {
        await VerificarClienteAsync(clienteId, tenantId);
        var fotos = await repository.GetAllByClienteAsync(clienteId, tenantId);
        return fotos.Select(ToDto);
    }

    public async Task<FotoDto> SubirAsync(Guid clienteId, UploadFotoDto dto, Guid tenantId)
    {
        await VerificarClienteAsync(clienteId, tenantId);

        var folder = $"nutrieval/{tenantId}/{clienteId}/fotos";
        var (url, publicId) = await cloudinaryService.SubirAsync(dto.Archivo!, folder);

        var foto = new FotoProgreso
        {
            Id            = Guid.NewGuid(),
            TenantId      = tenantId,
            ClienteId     = clienteId,
            UrlCloudinary = url,
            PublicId      = publicId,
            Tipo          = dto.Tipo,
            Fecha         = dto.Fecha ?? DateOnly.FromDateTime(DateTime.UtcNow),
            MesReferencia = dto.MesReferencia,
            SubidoPor     = "entrenador"
        };

        await repository.AddAsync(foto);

        logger.LogInformation("Foto subida: {Id} para cliente {ClienteId}", foto.Id, clienteId);

        return ToDto(foto);
    }

    public async Task EliminarAsync(Guid id, Guid clienteId, Guid tenantId)
    {
        var foto = await repository.GetByIdAsync(id, clienteId, tenantId)
            ?? throw new KeyNotFoundException("Foto no encontrada.");

        await cloudinaryService.EliminarAsync(foto.PublicId);
        await repository.DeleteAsync(foto);

        logger.LogInformation("Foto eliminada: {Id}", id);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task VerificarClienteAsync(Guid clienteId, Guid tenantId)
    {
        var existe = await clienteRepository.GetByIdAsync(clienteId, tenantId);
        if (existe is null)
            throw new KeyNotFoundException("Cliente no encontrado.");
    }

    // ── Mapper ────────────────────────────────────────────────────────────────

    private static FotoDto ToDto(FotoProgreso f) => new()
    {
        Id            = f.Id,
        TenantId      = f.TenantId,
        ClienteId     = f.ClienteId,
        UrlCloudinary = f.UrlCloudinary,
        PublicId      = f.PublicId,
        Tipo          = f.Tipo,
        Fecha         = f.Fecha,
        MesReferencia = f.MesReferencia,
        SubidoPor     = f.SubidoPor,
        CreatedAt     = f.CreatedAt
    };
}

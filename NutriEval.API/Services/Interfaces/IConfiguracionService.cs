using NutriEval.API.Models.DTOs.Configuracion;

namespace NutriEval.API.Services.Interfaces;

public interface IConfiguracionService
{
    Task<ConfiguracionDto> GetAsync(Guid tenantId);
    Task<ConfiguracionDto> UpdateRedesAsync(Guid tenantId, UpdateRedesDto dto);
}

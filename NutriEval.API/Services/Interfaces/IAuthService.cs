using NutriEval.API.Models.DTOs.Auth;

namespace NutriEval.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterEntrenadorAsync(RegisterEntrenadorDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto dto);
    Task<MeDto> GetMeAsync(Guid userId, string tipo);
}

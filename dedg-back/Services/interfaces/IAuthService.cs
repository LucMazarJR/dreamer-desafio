using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
}

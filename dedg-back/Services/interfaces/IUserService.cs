using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponseDto>> GetUsersAsync(int? managerId = null);
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto);
    Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto dto);
    Task DeleteUserAsync(int id);
}

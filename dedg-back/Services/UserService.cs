using Microsoft.EntityFrameworkCore;
using dedg_back.Data;
using dedg_back.Models.Entities;
using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .Select(u => MapToResponseDto(u))
            .ToListAsync();

        return users;
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null || !user.IsActive)
            return null;

        return MapToResponseDto(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = HashPassword(dto.Password),
            Role = dto.Role,
            Cpf = dto.Cpf,
            TimeZone = dto.TimeZone,
            ManagerId = dto.ManagerId,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created: {UserId}", user.Id);

        return MapToResponseDto(user);
    }

    public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            throw new InvalidOperationException($"User with id {id} not found");

        if (dto.Name != null)
            user.Name = dto.Name;
        if (dto.Email != null)
            user.Email = dto.Email;
        if (dto.TimeZone != null)
            user.TimeZone = dto.TimeZone;
        if (dto.ManagerId != null)
            user.ManagerId = dto.ManagerId;
        if (dto.IsActive.HasValue)
            user.IsActive = dto.IsActive.Value;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User updated: {UserId}", id);

        return MapToResponseDto(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            throw new InvalidOperationException($"User with id {id} not found");

        user.IsActive = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deleted (soft delete): {UserId}", id);
    }

    private static UserResponseDto MapToResponseDto(User user) =>
        new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Cpf = user.Cpf,
            TimeZone = user.TimeZone,
            ManagerId = user.ManagerId,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

    private static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public static bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}

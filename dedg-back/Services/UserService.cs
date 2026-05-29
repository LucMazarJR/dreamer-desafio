using Microsoft.EntityFrameworkCore;
using dedg_back.Data;
using dedg_back.Exceptions;
using dedg_back.Models.Entities;
using dedg_back.Models.Enums;
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

    public async Task<IEnumerable<UserResponseDto>> GetUsersAsync(int? managerId = null)
    {
        var query = _context.Users.Where(u => u.IsActive);

        if (managerId.HasValue)
            query = query.Where(u => u.ManagerId == managerId.Value);

        return await query
            .Select(u => MapToResponseDto(u))
            .ToListAsync();
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
        var emailInUse = await _context.Users
            .AnyAsync(u => u.Email == dto.Email && u.IsActive);

        if (emailInUse)
            throw new InvalidOperationException("Já existe um usuário ativo com este e-mail.");

        if (dto.ManagerId.HasValue)
        {
            var manager = await _context.Users.FindAsync(dto.ManagerId.Value);

            if (manager == null || !manager.IsActive)
                throw new InvalidOperationException("Gestor informado não encontrado.");

            if (manager.Role == UserRole.Collaborator)
                throw new InvalidOperationException("O usuário informado como gestor não possui permissão de gestão.");
        }

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

        if (user == null || !user.IsActive)
            throw new NotFoundException($"Usuário {id} não encontrado.");

        if (dto.Email != null)
        {
            var emailInUse = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.IsActive && u.Id != id);

            if (emailInUse)
                throw new InvalidOperationException("Já existe um usuário ativo com este e-mail.");
        }

        if (dto.ManagerId.HasValue)
        {
            var manager = await _context.Users.FindAsync(dto.ManagerId.Value);

            if (manager == null || !manager.IsActive)
                throw new InvalidOperationException("Gestor informado não encontrado.");

            if (manager.Role == UserRole.Collaborator)
                throw new InvalidOperationException("O usuário informado como gestor não possui permissão de gestão.");
        }

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

        if (user == null || !user.IsActive)
            throw new NotFoundException($"Usuário {id} não encontrado.");

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

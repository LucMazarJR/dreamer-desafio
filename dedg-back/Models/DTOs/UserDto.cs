using dedg_back.Models.Enums;

namespace dedg_back.Models.DTOs;

public class CreateUserDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserRole Role { get; set; }
    public string? Cpf { get; set; }
    public required string TimeZone { get; set; }
    public int? ManagerId { get; set; }
}

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? TimeZone { get; set; }
    public int? ManagerId { get; set; }
    public bool? IsActive { get; set; }
}

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Cpf { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

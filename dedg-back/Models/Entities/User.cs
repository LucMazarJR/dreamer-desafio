using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dedg_back.Models.Enums;

namespace dedg_back.Models.Entities;

[Table("tb_user")]
public class User
{
    [Key]
    [Column("id_user")]
    public int Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("email")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("role")]
    public UserRole Role { get; set; }

    [Column("cpf")]
    [MaxLength(14)]
    public string? Cpf { get; set; }

    [Required]
    [Column("time_zone")]
    [MaxLength(50)]
    public string TimeZone { get; set; } = string.Empty;

    [Column("manager_id")]
    public int? ManagerId { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? Manager { get; set; }
    public ICollection<User> Subordinates { get; set; } = [];
    public ICollection<TimeEvent> TimeEvents { get; set; } = [];
}

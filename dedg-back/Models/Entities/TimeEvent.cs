using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dedg_back.Models.Enums;

namespace dedg_back.Models.Entities;

[Table("tb_time_event")]
public class TimeEvent
{
    [Key]
    [Column("id_time_event")]
    public int Id { get; set; }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [Required]
    [Column("event_type")]
    public EventType EventType { get; set; }

    [Required]
    [Column("recorded_at")]
    public DateTime RecordedAt { get; set; }

    [Required]
    [Column("timezone_at_recording")]
    [MaxLength(50)]
    public string TimezoneAtRecording { get; set; } = string.Empty;

    [Column("is_travel")]
    public bool IsTravel { get; set; } = false;

    [Column("observation")]
    [MaxLength(300)]
    public string? Observation { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
}

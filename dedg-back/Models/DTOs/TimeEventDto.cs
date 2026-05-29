using dedg_back.Models.Enums;

namespace dedg_back.Models.DTOs;

public class CreateTimeEventDto
{
    public required int UserId { get; set; }
    public required EventType EventType { get; set; }
    public required DateTime RecordedAt { get; set; }
    public required string TimezoneAtRecording { get; set; }
    public bool IsTravel { get; set; } = false;
    public string? Observation { get; set; }
}

public class UpdateTimeEventDto
{
    public EventType? EventType { get; set; }
    public DateTime? RecordedAt { get; set; }
    public string? TimezoneAtRecording { get; set; }
    public bool? IsTravel { get; set; }
    public string? Observation { get; set; }
}

public class TimeEventResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public EventType EventType { get; set; }
    public DateTime RecordedAt { get; set; }
    public string TimezoneAtRecording { get; set; } = string.Empty;
    public bool IsTravel { get; set; }
    public string? Observation { get; set; }
    public DateTime CreatedAt { get; set; }
}

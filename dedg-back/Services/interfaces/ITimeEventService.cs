using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public interface ITimeEventService
{
    Task<IEnumerable<TimeEventResponseDto>> GetTimeEventsAsync(int? userId = null);
    Task<TimeEventResponseDto?> GetTimeEventByIdAsync(int id);
    Task<TimeEventResponseDto> CreateTimeEventAsync(CreateTimeEventDto dto);
    Task<TimeEventResponseDto> UpdateTimeEventAsync(int id, UpdateTimeEventDto dto);
    Task DeleteTimeEventAsync(int id);
    Task<TimeEventSummaryDto> GetSummaryAsync(int userId, int year, int month);
}

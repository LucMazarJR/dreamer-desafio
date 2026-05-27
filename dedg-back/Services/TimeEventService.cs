using Microsoft.EntityFrameworkCore;
using dedg_back.Data;
using dedg_back.Models.Entities;
using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public class TimeEventService : ITimeEventService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TimeEventService> _logger;

    public TimeEventService(AppDbContext context, ILogger<TimeEventService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<TimeEventResponseDto>> GetTimeEventsAsync(int? userId = null)
    {
        IQueryable<TimeEvent> query = _context.TimeEvents.AsQueryable();

        if (userId.HasValue)
            query = query.Where(te => te.UserId == userId.Value);

        var events = await query
            .OrderByDescending(te => te.RecordedAt)
            .Select(te => MapToResponseDto(te))
            .ToListAsync();

        return events;
    }

    public async Task<TimeEventResponseDto?> GetTimeEventByIdAsync(int id)
    {
        var timeEvent = await _context.TimeEvents.FindAsync(id);

        if (timeEvent == null)
            return null;

        return MapToResponseDto(timeEvent);
    }

    public async Task<TimeEventResponseDto> CreateTimeEventAsync(CreateTimeEventDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
            throw new InvalidOperationException($"User with id {dto.UserId} not found");

        var timeEvent = new TimeEvent
        {
            UserId = dto.UserId,
            EventType = dto.EventType,
            RecordedAt = dto.RecordedAt,
            TimezoneAtRecording = dto.TimezoneAtRecording,
            IsTravel = dto.IsTravel,
            Observation = dto.Observation
        };

        _context.TimeEvents.Add(timeEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("TimeEvent created: {TimeEventId} for User: {UserId}", timeEvent.Id, dto.UserId);

        return MapToResponseDto(timeEvent);
    }

    public async Task<TimeEventResponseDto> UpdateTimeEventAsync(int id, UpdateTimeEventDto dto)
    {
        var timeEvent = await _context.TimeEvents.FindAsync(id);

        if (timeEvent == null)
            throw new InvalidOperationException($"TimeEvent with id {id} not found");

        if (dto.EventType.HasValue)
            timeEvent.EventType = dto.EventType.Value;
        if (dto.RecordedAt.HasValue)
            timeEvent.RecordedAt = dto.RecordedAt.Value;
        if (dto.TimezoneAtRecording != null)
            timeEvent.TimezoneAtRecording = dto.TimezoneAtRecording;
        if (dto.IsTravel.HasValue)
            timeEvent.IsTravel = dto.IsTravel.Value;
        if (dto.Observation != null)
            timeEvent.Observation = dto.Observation;

        _context.TimeEvents.Update(timeEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("TimeEvent updated: {TimeEventId}", id);

        return MapToResponseDto(timeEvent);
    }

    public async Task DeleteTimeEventAsync(int id)
    {
        var timeEvent = await _context.TimeEvents.FindAsync(id);

        if (timeEvent == null)
            throw new InvalidOperationException($"TimeEvent with id {id} not found");

        _context.TimeEvents.Remove(timeEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("TimeEvent deleted: {TimeEventId}", id);
    }

    private static TimeEventResponseDto MapToResponseDto(TimeEvent timeEvent) =>
        new()
        {
            Id = timeEvent.Id,
            UserId = timeEvent.UserId,
            EventType = timeEvent.EventType,
            RecordedAt = timeEvent.RecordedAt,
            TimezoneAtRecording = timeEvent.TimezoneAtRecording,
            IsTravel = timeEvent.IsTravel,
            Observation = timeEvent.Observation,
            CreatedAt = timeEvent.CreatedAt
        };
}

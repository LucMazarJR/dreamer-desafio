using Microsoft.EntityFrameworkCore;
using dedg_back.Data;
using dedg_back.Exceptions;
using dedg_back.Models.Entities;
using dedg_back.Models.DTOs;
using dedg_back.Models.Enums;

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

        if (user == null || !user.IsActive)
            throw new NotFoundException($"Usuário {dto.UserId} não encontrado.");

        await ValidatePeriodIsNotClosed(dto.RecordedAt);

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
            throw new NotFoundException($"Registro de ponto {id} não encontrado.");

        var recordedAt = dto.RecordedAt ?? timeEvent.RecordedAt;
        await ValidatePeriodIsNotClosed(recordedAt);

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
            throw new NotFoundException($"Registro de ponto {id} não encontrado.");

        await ValidatePeriodIsNotClosed(timeEvent.RecordedAt);

        _context.TimeEvents.Remove(timeEvent);
        await _context.SaveChangesAsync();

        _logger.LogInformation("TimeEvent deleted: {TimeEventId}", id);
    }

    public async Task<TimeEventSummaryDto> GetSummaryAsync(int userId, int year, int month)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null || !user.IsActive)
            throw new NotFoundException($"Usuário {userId} não encontrado.");

        var events = await _context.TimeEvents
            .Where(te => te.UserId == userId
                      && te.RecordedAt.Year == year
                      && te.RecordedAt.Month == month)
            .OrderBy(te => te.RecordedAt)
            .ToListAsync();

        var period = await _context.MonthlyPeriods
            .FirstOrDefaultAsync(mp => mp.Year == year && mp.Month == month);

        var byDay = events.GroupBy(e => e.RecordedAt.Date);

        int totalWorkedMinutes = 0;
        int daysWorked = 0;

        foreach (var day in byDay)
        {
            var dayEvents = day.OrderBy(e => e.RecordedAt).ToList();

            DateTime? entryTime = null;
            DateTime? breakStartTime = null;
            int dayBreakMinutes = 0;
            int dayWorkedMinutes = 0;

            foreach (var evt in dayEvents)
            {
                switch (evt.EventType)
                {
                    case EventType.Entry:
                        entryTime = evt.RecordedAt;
                        break;
                    case EventType.BreakStart:
                        breakStartTime = evt.RecordedAt;
                        break;
                    case EventType.BreakEnd:
                        if (breakStartTime.HasValue)
                        {
                            dayBreakMinutes += (int)(evt.RecordedAt - breakStartTime.Value).TotalMinutes;
                            breakStartTime = null;
                        }
                        break;
                    case EventType.Exit:
                        if (entryTime.HasValue)
                        {
                            dayWorkedMinutes = (int)(evt.RecordedAt - entryTime.Value).TotalMinutes - dayBreakMinutes;
                            entryTime = null;
                        }
                        break;
                }
            }

            if (dayWorkedMinutes > 0)
            {
                totalWorkedMinutes += dayWorkedMinutes;
                daysWorked++;
            }
        }

        const int expectedMinutesPerDay = 8 * 60;
        int totalExpectedMinutes = daysWorked * expectedMinutesPerDay;
        int balance = totalWorkedMinutes - totalExpectedMinutes;

        return new TimeEventSummaryDto
        {
            UserId = userId,
            Year = year,
            Month = month,
            DaysWorked = daysWorked,
            TotalWorkedMinutes = totalWorkedMinutes,
            TotalExpectedMinutes = totalExpectedMinutes,
            OvertimeMinutes = balance > 0 ? balance : 0,
            NegativeMinutes = balance < 0 ? Math.Abs(balance) : 0,
            PeriodStatus = period?.Status.ToString() ?? "NoPeriod"
        };
    }

    private async Task ValidatePeriodIsNotClosed(DateTime recordedAt)
    {
        var periodClosed = await _context.MonthlyPeriods
            .AnyAsync(mp => mp.Year == recordedAt.Year
                         && mp.Month == recordedAt.Month
                         && mp.Status == PeriodStatus.Closed);

        if (periodClosed)
            throw new InvalidOperationException(
                $"O período {recordedAt:MM/yyyy} está fechado e não permite alterações.");
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

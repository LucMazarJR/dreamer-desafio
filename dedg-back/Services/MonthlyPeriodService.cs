using Microsoft.EntityFrameworkCore;
using dedg_back.Data;
using dedg_back.Models.Entities;
using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public class MonthlyPeriodService : IMonthlyPeriodService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MonthlyPeriodService> _logger;

    public MonthlyPeriodService(AppDbContext context, ILogger<MonthlyPeriodService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<MonthlyPeriodResponseDto>> GetMonthlyPeriodsAsync()
    {
        var periods = await _context.MonthlyPeriods
            .OrderByDescending(mp => mp.Year)
            .ThenByDescending(mp => mp.Month)
            .Select(mp => MapToResponseDto(mp))
            .ToListAsync();

        return periods;
    }

    public async Task<MonthlyPeriodResponseDto?> GetMonthlyPeriodByIdAsync(int id)
    {
        var period = await _context.MonthlyPeriods.FindAsync(id);

        if (period == null)
            return null;

        return MapToResponseDto(period);
    }

    public async Task<MonthlyPeriodResponseDto?> GetMonthlyPeriodByDateAsync(int year, int month)
    {
        var period = await _context.MonthlyPeriods
            .FirstOrDefaultAsync(mp => mp.Year == year && mp.Month == month);

        if (period == null)
            return null;

        return MapToResponseDto(period);
    }

    public async Task<MonthlyPeriodResponseDto> CreateMonthlyPeriodAsync(CreateMonthlyPeriodDto dto)
    {
        if (dto.Month < 1 || dto.Month > 12)
            throw new InvalidOperationException("Month must be between 1 and 12");

        var existingPeriod = await _context.MonthlyPeriods
            .FirstOrDefaultAsync(mp => mp.Year == dto.Year && mp.Month == dto.Month);

        if (existingPeriod != null)
            throw new InvalidOperationException("Period already exists");

        var period = new MonthlyPeriod
        {
            Year = dto.Year,
            Month = dto.Month
        };

        _context.MonthlyPeriods.Add(period);
        await _context.SaveChangesAsync();

        _logger.LogInformation("MonthlyPeriod created: {Year}/{Month}", dto.Year, dto.Month);

        return MapToResponseDto(period);
    }

    public async Task<MonthlyPeriodResponseDto> UpdateMonthlyPeriodAsync(int id, UpdateMonthlyPeriodDto dto)
    {
        var period = await _context.MonthlyPeriods.FindAsync(id);

        if (period == null)
            throw new InvalidOperationException($"MonthlyPeriod with id {id} not found");

        if (dto.Status.HasValue)
            period.Status = dto.Status.Value;
        if (dto.ClosedById.HasValue)
            period.ClosedById = dto.ClosedById.Value;

        _context.MonthlyPeriods.Update(period);
        await _context.SaveChangesAsync();

        _logger.LogInformation("MonthlyPeriod updated: {Id}", id);

        return MapToResponseDto(period);
    }

    public async Task DeleteMonthlyPeriodAsync(int id)
    {
        var period = await _context.MonthlyPeriods.FindAsync(id);

        if (period == null)
            throw new InvalidOperationException($"MonthlyPeriod with id {id} not found");

        _context.MonthlyPeriods.Remove(period);
        await _context.SaveChangesAsync();

        _logger.LogInformation("MonthlyPeriod deleted: {Id}", id);
    }

    private static MonthlyPeriodResponseDto MapToResponseDto(MonthlyPeriod period) =>
        new()
        {
            Id = period.Id,
            Year = period.Year,
            Month = period.Month,
            Status = period.Status,
            ClosedById = period.ClosedById,
            ClosedAt = period.ClosedAt
        };
}

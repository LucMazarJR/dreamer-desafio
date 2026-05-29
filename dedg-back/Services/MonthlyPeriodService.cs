using Microsoft.EntityFrameworkCore;
using dedg_back.Data;
using dedg_back.Exceptions;
using dedg_back.Models.Entities;
using dedg_back.Models.DTOs;
using dedg_back.Models.Enums;

namespace dedg_back.Services;

public class MonthlyPeriodService : IMonthlyPeriodService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MonthlyPeriodService> _logger;

    // Transições de status permitidas
    private static readonly Dictionary<PeriodStatus, PeriodStatus[]> AllowedTransitions = new()
    {
        [PeriodStatus.Open]     = [PeriodStatus.InReview, PeriodStatus.Closed],
        [PeriodStatus.InReview] = [PeriodStatus.Closed],
        [PeriodStatus.Closed]   = []
    };

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
            throw new InvalidOperationException("O mês deve ser entre 1 e 12.");

        var existingPeriod = await _context.MonthlyPeriods
            .FirstOrDefaultAsync(mp => mp.Year == dto.Year && mp.Month == dto.Month);

        if (existingPeriod != null)
            throw new InvalidOperationException($"Já existe um período para {dto.Month:D2}/{dto.Year}.");

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
            throw new NotFoundException($"Período {id} não encontrado.");

        if (dto.Status.HasValue)
        {
            var allowed = AllowedTransitions[period.Status];

            if (!allowed.Contains(dto.Status.Value))
                throw new InvalidOperationException(
                    $"Transição de '{period.Status}' para '{dto.Status.Value}' não é permitida.");

            if (dto.Status.Value == PeriodStatus.Closed)
            {
                if (!dto.ClosedById.HasValue)
                    throw new InvalidOperationException("ClosedById é obrigatório ao fechar o período.");

                period.ClosedAt = DateTime.UtcNow;
            }

            period.Status = dto.Status.Value;
        }

        if (dto.ClosedById.HasValue)
            period.ClosedById = dto.ClosedById.Value;

        _context.MonthlyPeriods.Update(period);
        await _context.SaveChangesAsync();

        _logger.LogInformation("MonthlyPeriod updated: {Id} -> Status: {Status}", id, period.Status);

        return MapToResponseDto(period);
    }

    public async Task DeleteMonthlyPeriodAsync(int id)
    {
        var period = await _context.MonthlyPeriods.FindAsync(id);

        if (period == null)
            throw new NotFoundException($"Período {id} não encontrado.");

        if (period.Status == PeriodStatus.Closed)
            throw new InvalidOperationException("Não é possível excluir um período já fechado.");

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

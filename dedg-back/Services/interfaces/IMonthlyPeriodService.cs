using dedg_back.Models.DTOs;

namespace dedg_back.Services;

public interface IMonthlyPeriodService
{
    Task<IEnumerable<MonthlyPeriodResponseDto>> GetMonthlyPeriodsAsync();
    Task<MonthlyPeriodResponseDto?> GetMonthlyPeriodByIdAsync(int id);
    Task<MonthlyPeriodResponseDto?> GetMonthlyPeriodByDateAsync(int year, int month);
    Task<MonthlyPeriodResponseDto> CreateMonthlyPeriodAsync(CreateMonthlyPeriodDto dto);
    Task<MonthlyPeriodResponseDto> UpdateMonthlyPeriodAsync(int id, UpdateMonthlyPeriodDto dto);
    Task DeleteMonthlyPeriodAsync(int id);
}

using dedg_back.Models.Enums;

namespace dedg_back.Models.DTOs;

public class CreateMonthlyPeriodDto
{
    public required int Year { get; set; }
    public required int Month { get; set; }
}

public class UpdateMonthlyPeriodDto
{
    public PeriodStatus? Status { get; set; }
    public int? ClosedById { get; set; }
}

public class MonthlyPeriodResponseDto
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public PeriodStatus Status { get; set; }
    public int? ClosedById { get; set; }
    public DateTime? ClosedAt { get; set; }
}

namespace dedg_back.Models.DTOs;

public class TimeEventSummaryDto
{
    public int UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int DaysWorked { get; set; }
    public int TotalWorkedMinutes { get; set; }
    public int TotalExpectedMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    public int NegativeMinutes { get; set; }
    public string PeriodStatus { get; set; } = string.Empty;
}

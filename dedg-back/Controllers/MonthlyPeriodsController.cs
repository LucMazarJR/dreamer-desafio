using Microsoft.AspNetCore.Mvc;
using dedg_back.Services;
using dedg_back.Models.DTOs;

namespace dedg_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonthlyPeriodsController : ControllerBase
{
    private readonly IMonthlyPeriodService _monthlyPeriodService;

    public MonthlyPeriodsController(IMonthlyPeriodService monthlyPeriodService)
    {
        _monthlyPeriodService = monthlyPeriodService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MonthlyPeriodResponseDto>>> GetMonthlyPeriods()
    {
        var periods = await _monthlyPeriodService.GetMonthlyPeriodsAsync();
        return Ok(periods);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MonthlyPeriodResponseDto>> GetMonthlyPeriod(int id)
    {
        var period = await _monthlyPeriodService.GetMonthlyPeriodByIdAsync(id);

        if (period == null)
            return NotFound();

        return Ok(period);
    }

    [HttpGet("by-date/{year}/{month}")]
    public async Task<ActionResult<MonthlyPeriodResponseDto>> GetMonthlyPeriodByDate(int year, int month)
    {
        var period = await _monthlyPeriodService.GetMonthlyPeriodByDateAsync(year, month);

        if (period == null)
            return NotFound();

        return Ok(period);
    }

    [HttpPost]
    public async Task<ActionResult<MonthlyPeriodResponseDto>> CreateMonthlyPeriod(CreateMonthlyPeriodDto dto)
    {
        try
        {
            var period = await _monthlyPeriodService.CreateMonthlyPeriodAsync(dto);
            return CreatedAtAction(nameof(GetMonthlyPeriod), new { id = period.Id }, period);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMonthlyPeriod(int id, UpdateMonthlyPeriodDto dto)
    {
        try
        {
            var period = await _monthlyPeriodService.UpdateMonthlyPeriodAsync(id, dto);
            return Ok(period);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMonthlyPeriod(int id)
    {
        try
        {
            await _monthlyPeriodService.DeleteMonthlyPeriodAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}

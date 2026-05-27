using Microsoft.AspNetCore.Authorization;
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

    // Qualquer autenticado pode consultar períodos
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MonthlyPeriodResponseDto>>> GetMonthlyPeriods()
    {
        var periods = await _monthlyPeriodService.GetMonthlyPeriodsAsync();
        return Ok(periods);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<MonthlyPeriodResponseDto>> GetMonthlyPeriod(int id)
    {
        var period = await _monthlyPeriodService.GetMonthlyPeriodByIdAsync(id);

        if (period == null)
            return NotFound();

        return Ok(period);
    }

    [HttpGet("by-date/{year}/{month}")]
    [Authorize]
    public async Task<ActionResult<MonthlyPeriodResponseDto>> GetMonthlyPeriodByDate(int year, int month)
    {
        var period = await _monthlyPeriodService.GetMonthlyPeriodByDateAsync(year, month);

        if (period == null)
            return NotFound();

        return Ok(period);
    }

    // Abertura de períodos mensais é responsabilidade do RH
    [HttpPost]
    [Authorize(Roles = "HrAdmin")]
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

    // Gestor pode mover para InReview; RH pode fechar (Closed)
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,HrAdmin")]
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
    [Authorize(Roles = "HrAdmin")]
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

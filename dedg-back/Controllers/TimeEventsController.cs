using Microsoft.AspNetCore.Mvc;
using dedg_back.Services;
using dedg_back.Models.DTOs;

namespace dedg_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeEventsController : ControllerBase
{
    private readonly ITimeEventService _timeEventService;

    public TimeEventsController(ITimeEventService timeEventService)
    {
        _timeEventService = timeEventService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TimeEventResponseDto>>> GetTimeEvents([FromQuery] int? userId)
    {
        var events = await _timeEventService.GetTimeEventsAsync(userId);
        return Ok(events);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TimeEventResponseDto>> GetTimeEvent(int id)
    {
        var timeEvent = await _timeEventService.GetTimeEventByIdAsync(id);

        if (timeEvent == null)
            return NotFound();

        return Ok(timeEvent);
    }

    [HttpPost]
    public async Task<ActionResult<TimeEventResponseDto>> CreateTimeEvent(CreateTimeEventDto dto)
    {
        try
        {
            var timeEvent = await _timeEventService.CreateTimeEventAsync(dto);
            return CreatedAtAction(nameof(GetTimeEvent), new { id = timeEvent.Id }, timeEvent);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTimeEvent(int id, UpdateTimeEventDto dto)
    {
        try
        {
            var timeEvent = await _timeEventService.UpdateTimeEventAsync(id, dto);
            return Ok(timeEvent);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTimeEvent(int id)
    {
        try
        {
            await _timeEventService.DeleteTimeEventAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}

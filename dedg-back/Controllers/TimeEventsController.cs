using Microsoft.AspNetCore.Authorization;
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

    // Todos os perfis podem consultar — o filtro por userId garante que colaborador
    // acesse apenas seus próprios registros na camada de negócio
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<TimeEventResponseDto>>> GetTimeEvents([FromQuery] int? userId)
    {
        var events = await _timeEventService.GetTimeEventsAsync(userId);
        return Ok(events);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TimeEventResponseDto>> GetTimeEvent(int id)
    {
        var timeEvent = await _timeEventService.GetTimeEventByIdAsync(id);

        if (timeEvent == null)
            return NotFound();

        return Ok(timeEvent);
    }

    // Colaborador registra o próprio ponto; gestor e RH também podem registrar
    [HttpPost]
    [Authorize]
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

    // Correção de registros é exclusiva de gestor e RH (regra de negócio)
    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,HrAdmin")]
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
    [Authorize(Roles = "Manager,HrAdmin")]
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

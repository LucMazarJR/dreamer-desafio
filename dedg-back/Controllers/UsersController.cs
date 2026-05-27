using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dedg_back.Exceptions;
using dedg_back.Services;
using dedg_back.Models.DTOs;

namespace dedg_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // Gestor e RH consultam usuários — colaborador vê apenas o próprio perfil via GET /{id}
    // Gestor filtra pelo próprio ID para ver apenas seu time: ?managerId={id}
    [HttpGet]
    [Authorize(Roles = "Manager,HrAdmin")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers([FromQuery] int? managerId)
    {
        var users = await _userService.GetUsersAsync(managerId);
        return Ok(users);
    }

    // Qualquer autenticado pode buscar por ID — útil para o colaborador ver o próprio perfil
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserResponseDto>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // Cadastro de colaboradores é responsabilidade do RH
    [HttpPost]
    [Authorize(Roles = "HrAdmin")]
    public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserDto dto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Fuso horário e dados do colaborador são gerenciados pelo RH
    [HttpPut("{id}")]
    [Authorize(Roles = "HrAdmin")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "HrAdmin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

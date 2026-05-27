using Microsoft.AspNetCore.Mvc;
using dedg_back.Services;
using dedg_back.Models.DTOs;

namespace dedg_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(result);
    }
}

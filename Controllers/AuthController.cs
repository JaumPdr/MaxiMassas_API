using MaxiMassas.DTOs.Auth;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realiza login e retorna o token JWT.
    /// Use o token retornado no botão "Authorize" do Swagger no formato: Bearer {token}
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var resultado = await _authService.LoginAsync(dto);
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Registra um novo usuário do sistema.
    /// Requer autenticação JWT (exceto quando não há nenhum usuário cadastrado).
    /// </summary>
    [HttpPost("registrar")]
    [AllowAnonymous] // Validação de auth feita manualmente abaixo para permitir o primeiro cadastro
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        // Permite cadastro sem token apenas se não existir nenhum usuário (bootstrap)
        bool temUsuarios = await _authService.TemUsuariosCadastradosAsync();
        if (temUsuarios && !User.Identity!.IsAuthenticated)
            return Forbid();

        try
        {
            var usuario = await _authService.RegistrarAsync(dto);
            return CreatedAtAction(nameof(GetUsuarios), new { }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Lista todos os usuários cadastrados.</summary>
    [HttpGet("usuarios")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _authService.GetUsuariosAsync();
        return Ok(usuarios);
    }
}

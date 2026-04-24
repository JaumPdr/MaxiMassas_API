using MaxiMassas.DTOs.Auth;
using MaxiMassas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMassas.Controllers;

[ApiController] // Define como controller de API
[Route("api/[controller]")] // Define rota base: api/auth
[Produces("application/json")] // Define retorno padrão como JSON
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;// Serviço responsável pelas regras de autenticação

    // Injeta o serviço de autenticação
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // Realiza login e retorna o token JWT.
    // Use o token retornado no botão "Authorize" do Swagger no formato: Bearer {token}
    [HttpPost("login")]// Endpoint: POST api/auth/login
    [AllowAnonymous] // Permite acesso sem autenticação
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)] // Retorno esperado em caso de sucesso
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Retorno esperado em caso de falha na autenticação
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var resultado = await _authService.LoginAsync(dto); // Realiza autenticação do usuário
            return Ok(resultado); // Retorna token JWT
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message }); // Retorna erro de autenticação
        }
    }

    // Registra um novo usuário do sistema.
    // Requer autenticação JWT (exceto quando não há nenhum usuário cadastrado).
    [HttpPost("registrar")] // Endpoint: POST api/auth/registrar
    [AllowAnonymous] // Permite acesso sem token apenas para o primeiro cadastro
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)] // Retorno esperado em caso de acesso negado
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto dto)
    {
        bool temUsuarios = await _authService.TemUsuariosCadastradosAsync(); // Verifica se já existem usuários cadastrados

        // Bloqueia cadastro sem autenticação caso já exista usuário
        if (temUsuarios && !User.Identity!.IsAuthenticated)
            return Forbid();

        try
        {
            var usuario = await _authService.RegistrarAsync(dto); // Realiza cadastro do usuário
            return CreatedAtAction(nameof(GetUsuarios), new { }, usuario); // Retorna usuário criado
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message }); // Retorna erro de regra de negócio
        }
    }

    //Lista todos os usuários cadastrados
    [HttpGet("usuarios")] // Endpoint: GET api/auth/usuarios
    [Authorize] // Exige autenticação JWT
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _authService.GetUsuariosAsync(); // Busca todos os usuários cadastrados
        return Ok(usuarios);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MaxiMassas.Config;
using MaxiMassas.DTOs.Auth;
using MaxiMassas.Entities;
using MaxiMassas.Repositories.Interfaces;
using MaxiMassas.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MaxiMassas.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUsuarioRepository usuarioRepository, IOptions<JwtSettings> jwtSettings)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Email ou senha inválidos.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos.");

        var token = GerarToken(usuario);
        var expiracao = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours);

        return new LoginResponseDto
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Expiracao = expiracao
        };
    }

    public async Task<UsuarioResponseDto> RegistrarAsync(RegistrarUsuarioDto dto)
    {
        if (await _usuarioRepository.EmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Já existe um usuário com este e-mail.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email.ToLower().Trim(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        };

        var criado = await _usuarioRepository.CreateAsync(usuario);

        return MapToDto(criado);
    }

    public async Task<IEnumerable<UsuarioResponseDto>> GetUsuariosAsync()
    {
        var usuarios = await _usuarioRepository.GetAllAsync();
        return usuarios.Select(MapToDto);
    }

    private string GerarToken(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UsuarioResponseDto MapToDto(Usuario u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email,
        Ativo = u.Ativo,
        CriadoEm = u.CriadoEm
    };
}

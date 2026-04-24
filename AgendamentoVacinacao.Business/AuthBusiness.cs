using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AgendamentoVacinacao.Business.Services;

public class AuthBusiness : IAuthBusiness
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthBusiness(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task RegistrarAsync(RegistroRequest request)
    {
        var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(request.Email);
        if (usuarioExistente != null)
            throw new InvalidOperationException("Este e-mail já está em uso.");


        string senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(request.Senha);

        var novoUsuario = new Usuario(request.Nome, request.Email, senhaCriptografada, request.Perfil);

        await _usuarioRepository.AdicionarAsync(novoUsuario);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

        var token = GerarTokenJwt(usuario);
        var refreshToken = GerarRefreshToken();

        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _usuarioRepository.AtualizarAsync(usuario);

        return new LoginResponse(token, refreshToken, usuario.Nome!, usuario.Email!, usuario.Perfil.ToString());
    }
    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(request.Email);

        if (usuario == null || usuario.RefreshToken != request.RefreshToken || usuario.RefreshTokenExpiryTime <= DateTime.Now)
            throw new UnauthorizedAccessException("Refresh Token inválido ou expirado. Faça login novamente.");

        var novoToken = GerarTokenJwt(usuario);
        var novoRefreshToken = GerarRefreshToken();

        usuario.RefreshToken = novoRefreshToken;
        usuario.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
        await _usuarioRepository.AtualizarAsync(usuario);

        return new LoginResponse(novoToken, novoRefreshToken, usuario.Nome!, usuario.Email!, usuario.Perfil.ToString());
    }
    private static string GerarRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private string GerarTokenJwt(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email!),
            new Claim(ClaimTypes.Name, usuario.Nome!),
            new Claim(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(double.Parse(_configuration["Jwt:ExpireHours"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
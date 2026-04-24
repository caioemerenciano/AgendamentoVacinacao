using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using Microsoft.AspNetCore.Mvc;


namespace AgendamentoVacinacao.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthBusiness _authBusiness;

    public AuthController(IAuthBusiness authBusiness)
    {
        _authBusiness = authBusiness;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
    {
        try
        {
            await _authBusiness.RegistrarAsync(request);
            return StatusCode(201, new { Mensagem = "Usuário cadastrado com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Erro = ex.Message });
        }
    }
    [HttpPost("login")]
    public async Task<IActionResult>Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authBusiness.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { Erro = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Erro = "Erro interno", Detalhe = ex.Message });
        }
    }
    
}

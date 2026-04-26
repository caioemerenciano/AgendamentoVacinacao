using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AgendamentoVacinacao.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoBusiness _agendendamentoBusiness;
    
    public AgendamentoController(IAgendamentoBusiness agendamentoBusiness)
    {
        _agendendamentoBusiness = agendamentoBusiness;
    }

    [HttpPost]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> CriarAgendamento([FromBody] CriarAgendamentoRequest request, [FromServices] FluentValidation.IValidator<CriarAgendamentoRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)  
        {
            return BadRequest(validationResult.Errors);
        }

        var usuarioId = GetUsuarioId();
        var response = await _agendendamentoBusiness.CriarAgendamentoAsync(request, usuarioId);
        return Created(string.Empty, response);
    }

    [HttpGet]
    public async Task<IActionResult> ListarAgendamentos()
    {
        var usuarioId = GetUsuarioId();
        var perfil = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        
        var response = await _agendendamentoBusiness.ObterTodosAsync(usuarioId, perfil);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var agendamento = await _agendendamentoBusiness.ObterPorIdAsync(id);
        if (agendamento == null) return NotFound();
        return Ok(agendamento);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAgendamentoRequest request)
    {
        var usuarioId = GetUsuarioId();
        var response = await _agendendamentoBusiness.AtualizarAgendamentoAsync(id, request, usuarioId);
        return Ok(response);
    }

    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var usuarioId = GetUsuarioId();
        var perfil = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        await _agendendamentoBusiness.CancelarAgendamentoAsync(id, usuarioId, perfil);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusRequest request)
    {
        await _agendendamentoBusiness.AtualizarStatusAsync(id, (StatusAgendamento)request.NovoStatus);
        return Ok(new { mensagem = "Status atualizado com sucesso!" });
    }

    [HttpPatch("{id}/realizar")]
    [Authorize(Roles = "Enfermeiro")]
    public async Task<IActionResult> Realizar(int id)
    {
        await _agendendamentoBusiness.AtualizarStatusAsync(id, StatusAgendamento.Realizado);
        return Ok(new { mensagem = "Agendamento realizado com sucesso!" });
    }

    private int GetUsuarioId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim == null || !int.TryParse(idClaim.Value, out int id))
        {
            throw new UnauthorizedAccessException("Usuário não identificado.");
        }
        return id;
    }

}

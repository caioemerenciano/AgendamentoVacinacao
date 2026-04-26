using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        var response = await _agendendamentoBusiness.CriarAgendamentoAsync(request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> ObterTodos()
    {
        var agendamentos = await _agendendamentoBusiness.ObterTodosAsync();
        return Ok(agendamentos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var agendamento = await _agendendamentoBusiness.ObterPorIdAsync(id);
        if (agendamento == null) return NotFound();
        return Ok(agendamento);
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAgendamentoRequest request)
    {
        var response = await _agendendamentoBusiness.AtualizarAgendamentoAsync(id, request);
        return Ok(response);
    }

    [HttpPatch("{id}/cancelar")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _agendendamentoBusiness.CancelarAgendamentoAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusRequest request)
    {
        await _agendendamentoBusiness.AtualizarStatusAsync(id, (StatusAgendamento)request.NovoStatus);
        return Ok(new { mensagem = "Status atualizado com sucesso!" });
    }

}

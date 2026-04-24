using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> CriarAgendamento([FromBody] CriarAgendamentoRequest request)
    {
        var response = await _agendendamentoBusiness.CriarAgendamentoAsync(request);

        return Created(string.Empty, response);
    }

    [HttpGet]
    [Authorize(Roles = "Enfermeiro")]
    public async Task<IActionResult> ObterTodos()
    {
        var agendamentos = await _agendendamentoBusiness.ObterTodosAsync();
        return Ok(agendamentos);
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarAgendamentoRequest request)
    {
        try
        {
            var response = await _agendendamentoBusiness.AtualizarAgendamentoAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id}/cancelar")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> Cancelar(int id)
    {
        try
        {
            await _agendendamentoBusiness.CancelarAgendamentoAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


}

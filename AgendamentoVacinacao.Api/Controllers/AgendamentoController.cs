using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.Enums;
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
    public async Task<IActionResult> CriarAgendamento([FromBody] CriarAgendamentoRequest request, [FromServices] FluentValidation.IValidator<CriarAgendamentoRequest> validator)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)  
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var response = await _agendendamentoBusiness.CriarAgendamentoAsync(request);
            return Created(string.Empty, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Paciente")]
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
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusRequest request)
    {
        try
        {
            await _agendendamentoBusiness.AtualizarStatusAsync(id, (StatusAgendamento)request.NovoStatus);

            return Ok(new { mensagem = "Status atualizado com sucesso!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

}

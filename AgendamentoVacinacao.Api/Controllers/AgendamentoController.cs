using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoBusiness _agendendamentoBusiness;
    
    public AgendamentoController(IAgendamentoBusiness agendamentoBusiness)
    {
        _agendendamentoBusiness = agendamentoBusiness;
    }

    [HttpPost]
    public async Task<IActionResult> CriarAgendamento([FromBody] CriarAgendamentoRequest request)
    {
        var response = await _agendendamentoBusiness.CriarAgendamentoAsync(request);

        return Created(string.Empty, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var agendamentos = await _agendendamentoBusiness.ObterTodosAsync();
        return Ok(agendamentos);
    }
}

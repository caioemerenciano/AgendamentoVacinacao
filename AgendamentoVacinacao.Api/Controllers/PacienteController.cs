using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendamentoVacinacao.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacienteController : ControllerBase
{
    private readonly IPacienteBusiness _pacienteBusiness;

    public PacienteController(IPacienteBusiness pacienteBusiness)
    {
        _pacienteBusiness = pacienteBusiness;
    }

    [HttpPost]
    public async Task<IActionResult> CriarPaciente([FromBody] CriarPacienteRequest request)
    {
        var response = await _pacienteBusiness.CriarPacienteAsync(request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    [Authorize(Roles = "Enfermeiro")]
    public async Task<IActionResult> GetAll()
    {
        var pacientes = await _pacienteBusiness.ObterTodosAsync();
        return Ok(pacientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var paciente = await _pacienteBusiness.ObterPorIdAsync(id);
        return paciente == null ? NotFound() : Ok(paciente);
    }
}

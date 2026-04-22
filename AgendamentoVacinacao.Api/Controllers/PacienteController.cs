using AgendamentoVacinacao.Business.Interface;
using AgendamentoVacinacao.Entity.DTOs.Request;
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

    public async Task<IActionResult> CriarPaciente([FromBody] CriarPacienteRequest request)
    {
        var response = await _pacienteBusiness.CriarPacienteAsync(request);

        return Created(string.Empty, response);
    }
}

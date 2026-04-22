using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;

namespace AgendamentoVacinacao.Business.Interface;

public interface IPacienteService
{
    Task<PacienteResponse> CriarPacienteAsync(CriarPacienteRequest request);
}
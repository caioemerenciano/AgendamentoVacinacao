using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;

namespace AgendamentoVacinacao.Business.Interface;

public interface IPacienteBusiness
{
    Task<PacienteResponse> CriarPacienteAsync(CriarPacienteRequest request);
    Task<IEnumerable<PacienteResponse>> ObterTodosAsync();
    Task<PacienteResponse?> ObterPorIdAsync(int id);
}
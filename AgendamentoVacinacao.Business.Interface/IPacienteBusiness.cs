using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;

namespace AgendamentoVacinacao.Business.Interface;

public interface IPacienteBusiness
{
    Task<AgendamentoResponse> CriarAgendamentoAsync(CriarPacienteRequest request);
}

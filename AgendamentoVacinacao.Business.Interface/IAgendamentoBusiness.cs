using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Enums;


namespace AgendamentoVacinacao.Business.Interface;

public interface IAgendamentoBusiness
{
    Task<AgendamentoResponse> CriarAgendamentoAsync(CriarAgendamentoRequest request);
    Task<AgendamentoResponse?> ObterPorIdAsync(int id);
    Task<IEnumerable<AgendamentoResponse>> ObterTodosAsync();
    Task<AgendamentoResponse> AtualizarAgendamentoAsync(int id, AtualizarAgendamentoRequest request);
    Task AtualizarStatusAsync(int id, StatusAgendamento novoStatus);
    Task CancelarAgendamentoAsync(int id);
}

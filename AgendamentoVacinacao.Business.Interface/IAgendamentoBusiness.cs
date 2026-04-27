using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;
using AgendamentoVacinacao.Entity.Enums;


namespace AgendamentoVacinacao.Business.Interface;

public interface IAgendamentoBusiness
{
    Task<AgendamentoResponse> CriarAgendamentoAsync(CriarAgendamentoRequest request, int usuarioId);
    Task<AgendamentoResponse?> ObterPorIdAsync(int id);
    Task<IEnumerable<AgendamentoResponse>> ObterTodosAsync(int usuarioId, string perfil);
    Task<AgendamentoResponse> AtualizarAgendamentoAsync(int id, AtualizarAgendamentoRequest request, int usuarioId);
    Task AtualizarStatusAsync(int id, StatusAgendamento novoStatus);
    Task CancelarAgendamentoAsync(int id, int usuarioId, string perfil);
}

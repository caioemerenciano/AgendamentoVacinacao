using AgendamentoVacinacao.Entity.DTOs.Request;
using AgendamentoVacinacao.Entity.DTOs.Response;


namespace AgendamentoVacinacao.Business.Interface;

public interface IAgendamentoBusiness
{
    Task<AgendamentoResponse> CriarAgendamentoAsync(CriarAgendamentoRequest request);
    Task<AgendamentoResponse?> ObterPorIdAsync(int id);
    Task<IEnumerable<AgendamentoResponse>> ObterTodosAsync();
}

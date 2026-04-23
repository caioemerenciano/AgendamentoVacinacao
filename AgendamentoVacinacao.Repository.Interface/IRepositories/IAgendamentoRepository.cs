using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories;

public interface IAgendamentoRepository
{
    Task<int> ContarAgendamentosPorDiaAsync(DateTime data);
    Task<int> ContarAgendamentosPorHorarioAsync(DateTime data, TimeSpan hora);
    Task<IEnumerable<Agendamento>> ObterTodosAsync();
    Task<Agendamento?> ObterPorIdAsync(int id);
    Task AtualizarAsync(Agendamento agendamento);
    Task<bool> ExisteAgendamentoConflitanteAsync(DateTime data, TimeSpan hora, int agendamentoIdIgnorado);

    
    Task AdicionarAsync(Agendamento agendamento);
    Task SalvarAlteracoesAsync();
}

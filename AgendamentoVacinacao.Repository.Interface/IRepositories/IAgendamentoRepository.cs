using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories;

public interface IAgendamentoRepository
{
    Task<int> ContarAgendamentosPorDiaAsync(DateTime data);
    Task<int> ContarAgendamentosPorHorarioAsync(DateTime dataHoraExata);

    Task AdicionarAsync(Agendamento agendamento);
}

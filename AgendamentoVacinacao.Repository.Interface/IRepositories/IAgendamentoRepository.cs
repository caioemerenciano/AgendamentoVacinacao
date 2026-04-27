using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories;

public interface IAgendamentoRepository : IBaseRepository<Agendamento>
{
    Task<int> ContarAgendamentosPorDiaAsync(DateTime data);
    Task<int> ContarAgendamentosPorHorarioAsync(DateTime data, TimeSpan hora);
    Task<bool> ExisteAgendamentoConflitanteAsync(DateTime data, TimeSpan hora, int agendamentoIdIgnorado);
    
    // ObterTodosAsync e ObterPorIdAsync foram removidos pois a base temGetAllAsync e GetByIdAsync.
    // Mas para manter compatibilidade sem refatorar TUDO agora, poderíamos manter aliases.
    // Porém o plano pede para "Remover APENAS os métodos de CRUD básico".
    // Vou remover os duplicados e atualizar as referências nos Services depois.
}

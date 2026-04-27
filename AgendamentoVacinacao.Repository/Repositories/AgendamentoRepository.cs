using AgendamentoVacinacao.Repository.Context;
using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Entity.Enums;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace AgendamentoVacinacao.Repository.Repositories;

public class AgendamentoRepository : BaseRepository<Agendamento>, IAgendamentoRepository
{
    public AgendamentoRepository(AgendamentoVacinacaoContext context) : base(context)
    {
    }

    public async Task<int> ContarAgendamentosPorDiaAsync(DateTime data) => 
        await _dbset.Agendamentos.CountAsync(a => a.DataAgendamento.Date == data && a.Status != StatusAgendamento.Cancelado);
    
    public async Task<int> ContarAgendamentosPorHorarioAsync(DateTime data, TimeSpan hora)
    {
        var limiteInferior = hora.Subtract(TimeSpan.FromHours(1));
        if (limiteInferior < TimeSpan.Zero) limiteInferior = TimeSpan.Zero;

        var limiteSuperior = hora.Add(TimeSpan.FromHours(1));
        if (limiteSuperior > new TimeSpan(23, 59, 59)) limiteSuperior = new TimeSpan(23, 59, 59);

        return await dbset.Agendamentos.CountAsync(a => a.DataAgendamento == data 
                                                       && a.HoraAgendamento > limiteInferior 
                                                       && a.HoraAgendamento < limiteSuperior
                                                       && a.Status != StatusAgendamento.Cancelado);
    }
   
    // Especialização com Include
    public override async Task<IEnumerable<Agendamento>> GetAllAsync() =>
        await _dbset.Agendamentos
            .Include(a => a.Paciente)
            .AsNoTracking()
            .ToListAsync();

    // Especialização com Include
    public override async Task<Agendamento?> GetByIdAsync(int id) =>
        await _dbset.Agendamentos
            .Include(a => a.Paciente)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<bool> ExisteAgendamentoConflitanteAsync(DateTime data, TimeSpan hora, int agendamentoIdIgnorado)
    {
        var limiteInferior = hora.Subtract(TimeSpan.FromHours(1));
        if (limiteInferior < TimeSpan.Zero) limiteInferior = TimeSpan.Zero;

        var limiteSuperior = hora.Add(TimeSpan.FromHours(1));
        if (limiteSuperior > new TimeSpan(23, 59, 59)) limiteSuperior = new TimeSpan(23, 59, 59);

        return await _dbset.Agendamentos
            .AnyAsync(a => a.DataAgendamento == data
                        && a.HoraAgendamento > limiteInferior
                        && a.HoraAgendamento < limiteSuperior
                        && a.Id != agendamentoIdIgnorado
                        && a.Status != StatusAgendamento.Cancelado);
    }
}

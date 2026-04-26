using AgendamentoVacinacao.Repository.Context;
using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using Microsoft.EntityFrameworkCore;


namespace AgendamentoVacinacao.Repository.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AgendamentoVacinacaoContext _context;

    public AgendamentoRepository(AgendamentoVacinacaoContext context)
    {
        _context = context;
    }
    public async Task<int> ContarAgendamentosPorDiaAsync(DateTime data) => 
        await _context.Agendamentos.CountAsync(a => a.DataAgendamento.Date == data);
    
    public async Task<int> ContarAgendamentosPorHorarioAsync(DateTime data, TimeSpan hora)
    {
        var limiteInferior = hora.Subtract(TimeSpan.FromHours(1));
        if (limiteInferior < TimeSpan.Zero) limiteInferior = TimeSpan.Zero;

        var limiteSuperior = hora.Add(TimeSpan.FromHours(1));
        if (limiteSuperior > new TimeSpan(23, 59, 59)) limiteSuperior = new TimeSpan(23, 59, 59);

        return await _context.Agendamentos.CountAsync(a => a.DataAgendamento == data && a.HoraAgendamento > limiteInferior && a.HoraAgendamento < limiteSuperior);
    }
   
    public async Task<IEnumerable<Agendamento>> ObterTodosAsync() =>
        await _context.Agendamentos
            .Include(a => a.Paciente)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Agendamento?> ObterPorIdAsync(int id) =>
        await _context.Agendamentos
            .Include(a => a.Paciente)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task AtualizarAsync(Agendamento agendamento)
    {
        _context.Agendamentos.Update(agendamento);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExisteAgendamentoConflitanteAsync(DateTime data, TimeSpan hora, int agendamentoIdIgnorado)
    {
        var limiteInferior = hora.Subtract(TimeSpan.FromHours(1));
        if (limiteInferior < TimeSpan.Zero) limiteInferior = TimeSpan.Zero;

        var limiteSuperior = hora.Add(TimeSpan.FromHours(1));
        if (limiteSuperior > new TimeSpan(23, 59, 59)) limiteSuperior = new TimeSpan(23, 59, 59);

        return await _context.Agendamentos
            .AnyAsync(a => a.DataAgendamento == data
                        && a.HoraAgendamento > limiteInferior
                        && a.HoraAgendamento < limiteSuperior
                        && a.Id != agendamentoIdIgnorado);
    }


    public async Task AdicionarAsync(Agendamento agendamento)
    {
        await _context.Agendamentos.AddAsync(agendamento);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
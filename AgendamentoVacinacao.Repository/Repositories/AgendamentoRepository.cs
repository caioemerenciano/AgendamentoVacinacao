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
    
    public async Task<int> ContarAgendamentosPorHorarioAsync(DateTime data, TimeSpan hora) => 
        await _context.Agendamentos.CountAsync(a => a.DataAgendamento == data && a.HoraAgendamento == hora);
   
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
        return await _context.Agendamentos
            .AnyAsync(a => a.DataAgendamento == data
                        && a.HoraAgendamento == hora
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
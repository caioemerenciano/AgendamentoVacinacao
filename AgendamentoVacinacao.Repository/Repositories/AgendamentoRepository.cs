using AgendamentoVacinacao.Repository.Context;
using AgendamentoVacinacao.Repository.Interface;
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

    public async Task<int> ContarAgendamentosPorDiaAsync(DateTime data)
    {
        return await _context.Agendamentos
            .CountAsync(a => a.DataAgendamento.Date == data.Date);
    }

    public async Task<int> ContarAgendamentosPorHorarioAsync(DateTime data, TimeSpan hora)
    {
        return await _context.Agendamentos
            .CountAsync(a => a.DataAgendamento == data && a.HoraAgendamento == hora);
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
using AgendamentoVacinacao.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Context;

public class AgendamentoVacinacaoContext : DbContext
{
    public AgendamentoVacinacaoContext(DbContextOptions<AgendamentoVacinacaoContext> options) : base(options)
    {
    }

    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Agendamento> Agendamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendamentoVacinacaoContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

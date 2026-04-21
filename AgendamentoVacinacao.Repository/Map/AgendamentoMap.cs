using AgendamentoVacinacao.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendamentoVacinacao.Repository.Map;

public class AgendamentoMap : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("tb_agendamento");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.IdPaciente)
            .HasColumnName("id_paciente")
            .IsRequired();

        builder.Property(a => a.DataAgendamento)
            .HasColumnName("data_agendamento")
            .IsRequired();


    }
}

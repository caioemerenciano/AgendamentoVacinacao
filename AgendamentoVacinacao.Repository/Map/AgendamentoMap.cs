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

        builder.Property(a => a.Id)
            .HasColumnName("id_agendamento")
            .UseIdentityColumn();

        builder.Property(a => a.IdPaciente)
            .HasColumnName("id_paciente")
            .IsRequired();

        builder.Property(a => a.DataAgendamento)
            .HasColumnName("dat_agendamento")
            .IsRequired();

        builder.Property(a => a.HoraAgendamento)
            .HasColumnName("hor_agendamento")
            .IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("dsc_status")
            .HasConversion<string>() 
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.DataCriacao)
            .HasColumnName("dat_criacao")
            .IsRequired();

        builder.HasOne(a => a.Paciente)
            .WithMany(p => p.Agendamentos)
            .HasForeignKey(a => a.IdPaciente);
    }
}
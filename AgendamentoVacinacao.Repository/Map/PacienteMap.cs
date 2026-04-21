using AgendamentoVacinacao.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendamentoVacinacao.Repository.Map;

public class PacienteMap : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("tb_paciente");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.Nome)
            .HasColumnName("dsc_nome")
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.DataNascimento)
            .HasColumnName("dat_nascimento")
            .IsRequired();
    }
}

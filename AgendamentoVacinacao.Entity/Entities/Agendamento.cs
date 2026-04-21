using System;

namespace AgendamentoVacinacao.Entity.Entities;

public class Agendamento
{
    protected Agendamento() { }

    public Agendamento(Guid id, Guid idPaciente, DateTime dataAgendamento)
    {
        Id = id;
        IdPaciente = idPaciente;
        DataAgendamento = dataAgendamento;
        CriadoEm = DateTime.Now; 
    }

    public Guid Id { get; private set; }
    public Guid IdPaciente { get; private set; }
    public DateTime DataAgendamento { get; private set; }
    public DateTime CriadoEm { get; private set; }


    public virtual Paciente? Paciente { get; private set; }
}
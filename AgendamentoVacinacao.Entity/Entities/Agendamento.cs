using System;
using AgendamentoVacinacao.Entity.Enums;
namespace AgendamentoVacinacao.Entity.Entities;

public class Agendamento
{
    protected Agendamento() { }

    public Agendamento(Guid id, Guid idPaciente, DateTime dataAgendamento, TimeSpan horaAgendamento, StatusAgendamento status)
    {
        Id = id;
        IdPaciente = idPaciente;
        DataAgendamento = dataAgendamento;
        HoraAgendamento = horaAgendamento;
        Status = status;
        DataCriacao = DateTime.Now; 
    }

    public Guid Id { get; private set; }
    public Guid IdPaciente { get; private set; }
    public DateTime DataAgendamento { get; private set; }
    public TimeSpan HoraAgendamento { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public DateTime DataCriacao { get; private set; }


    public virtual Paciente? Paciente { get; private set; }
}
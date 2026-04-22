using System;
using AgendamentoVacinacao.Entity.Enums;
namespace AgendamentoVacinacao.Entity.Entities;

public class Agendamento
{
    protected Agendamento() { }

    public Agendamento(int id, int idPaciente, DateTime dataAgendamento, TimeSpan horaAgendamento, StatusAgendamento status)
    {
        Id = id;
        IdPaciente = idPaciente;
        DataAgendamento = dataAgendamento;
        HoraAgendamento = horaAgendamento;
        Status = status;
        DataCriacao = DateTime.Now; 
    }

    public int Id { get; private set; }
    public int IdPaciente { get; private set; }
    public DateTime DataAgendamento { get; private set; }
    public TimeSpan HoraAgendamento { get; private set; }
    public StatusAgendamento Status { get; private set; }
    public DateTime DataCriacao { get; private set; }


    public virtual Paciente? Paciente { get; private set; }
}
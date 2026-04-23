using System;
using AgendamentoVacinacao.Entity.Enums;
namespace AgendamentoVacinacao.Entity.Entities;

public class Agendamento
{
    public Agendamento() { }

    public Agendamento(int id, int idPaciente, DateTime dataAgendamento, TimeSpan horaAgendamento, StatusAgendamento status)
    {
        Id = id;
        IdPaciente = idPaciente;
        DataAgendamento = dataAgendamento;
        HoraAgendamento = horaAgendamento;
        Status = status;
        DataCriacao = DateTime.Now; 
    }

    public int Id { get; set; }
    public int IdPaciente { get; set; }
    public DateTime DataAgendamento { get; set; }
    public TimeSpan HoraAgendamento { get; set; }
    public StatusAgendamento Status { get; set; }
    public DateTime DataCriacao { get; set; }


    public virtual Paciente? Paciente { get; set; }
}
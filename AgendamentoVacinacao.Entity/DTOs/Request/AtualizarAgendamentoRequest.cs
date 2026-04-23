namespace AgendamentoVacinacao.Entity.DTOs.Request;

public class AtualizarAgendamentoRequest
{
    public DateTime DataAgendamento { get; set; }
    public TimeSpan HoraAgendamento { get; set; }
}

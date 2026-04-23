namespace AgendamentoVacinacao.Entity.DTOs.Request;

public record CriarAgendamentoRequest(int IdPaciente, DateTime DataAgendamento, TimeSpan HoraAgendamento);


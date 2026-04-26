namespace AgendamentoVacinacao.Entity.DTOs.Request;

public record CriarAgendamentoRequest(
    string Nome,
    DateTime DataNascimento,
    DateTime DataAgendamento,
    TimeSpan HoraAgendamento 
);
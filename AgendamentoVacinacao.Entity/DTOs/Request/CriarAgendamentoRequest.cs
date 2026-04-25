namespace AgendamentoVacinacao.Entity.DTOs.Request;

public record CriarAgendamentoRequest(
    string Nome,
    string DataNascimento,
    string DataAgendamento,
    string Horario 
);
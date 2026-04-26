using AgendamentoVacinacao.Entity.Enums;

namespace AgendamentoVacinacao.Entity.DTOs.Response;

public record AgendamentoResponse(
    int Id,
    int IdPaciente,
    string NomePaciente,
    DateTime DataAgendamento,
    TimeSpan HoraAgendamento,
    StatusAgendamento Status);

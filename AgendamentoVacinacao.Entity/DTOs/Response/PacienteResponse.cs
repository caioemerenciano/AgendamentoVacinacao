namespace AgendamentoVacinacao.Entity.DTOs.Response;

public record PacienteResponse(
    int Id,
    string Nome,
    DateTime DataNascimento
);
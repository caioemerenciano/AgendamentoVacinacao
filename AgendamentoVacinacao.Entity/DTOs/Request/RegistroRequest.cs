using AgendamentoVacinacao.Entity.Enums;

namespace AgendamentoVacinacao.Entity.DTOs.Request;

public record RegistroRequest(string Email, string Senha);
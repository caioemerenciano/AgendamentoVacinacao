namespace AgendamentoVacinacao.Entity.DTOs.Response;

public record LoginResponse(string Token, string RefreshToken, string Nome, string Email, string Perfil);
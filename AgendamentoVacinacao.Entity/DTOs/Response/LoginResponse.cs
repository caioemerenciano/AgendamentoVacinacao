namespace AgendamentoVacinacao.Entity.DTOs.Response;

public record LoginResponse(int Id, string Token, string RefreshToken, string Nome, string Email, string Perfil);
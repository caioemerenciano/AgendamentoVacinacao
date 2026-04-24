using AgendamentoVacinacao.Entity.Enums;

namespace AgendamentoVacinacao.Entity.DTOs.Request;

public record RegistroRequest(string Nome, string Email, string Senha, PerfilUsuario Perfil);
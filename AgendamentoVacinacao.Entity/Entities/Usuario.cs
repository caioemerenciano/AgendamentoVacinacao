using AgendamentoVacinacao.Entity.Enums;

namespace AgendamentoVacinacao.Entity.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string? Nome { get; set; } 
    public string? Email { get; set; } 
    public string? SenhaHash { get; set; } 
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public PerfilUsuario Perfil { get; set; }
    

    public Usuario() { }

    public Usuario(string nome, string email, string senhaHash, PerfilUsuario perfil)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;
        Perfil = perfil;
    }
}

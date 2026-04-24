using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
}

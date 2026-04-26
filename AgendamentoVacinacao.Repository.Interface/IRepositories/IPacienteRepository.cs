using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories;

public interface IPacienteRepository
{
    Task<bool> ExistePacienteAsync(string nome, DateTime dataNascimento);
    Task<IEnumerable<Paciente>> ObterTodosAsync();
    Task<Paciente?> ObterPorIdAsync(int id);
    Task<Paciente?> ObterPorNomeEDataNascimentoAsync(string nome, DateTime dataNascimento);

    Task AdicionarAsync(Paciente paciente);
    Task AdicionarComIdForcadoAsync(Paciente paciente);
    Task SalvarAlteracoesAsync();
}
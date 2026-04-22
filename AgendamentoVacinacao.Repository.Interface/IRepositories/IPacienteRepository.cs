using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface;

public interface IPacienteRepository
{
    Task<bool> ExistePacienteAsync(string nome, DateTime dataNascimento);
    Task<IEnumerable<Paciente>> ObterTodosAsync();
    Task<Paciente?> ObterPorIdAsync(int id);


    Task AdicionarAsync(Paciente paciente);
    Task SalvarAlteracoesAsync();
}
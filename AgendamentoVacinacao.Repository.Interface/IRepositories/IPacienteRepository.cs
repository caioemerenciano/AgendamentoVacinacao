using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface.IRepositories;

public interface IPacienteRepository : IBaseRepository<Paciente>
{
    Task<bool> ExistePacienteAsync(string nome, DateTime dataNascimento);
    Task<Paciente?> ObterPorNomeEDataNascimentoAsync(string nome, DateTime dataNascimento);
    Task AdicionarComIdForcadoAsync(Paciente paciente);
}
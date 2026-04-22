using AgendamentoVacinacao.Entity.Entities;

namespace AgendamentoVacinacao.Repository.Interface;

public interface IPacienteRepository
{
    Task<bool> ExistePacienteAsync(string nome, DateTime dataNascimento);
    Task AdicionarAsync(Paciente paciente);
    Task SalvarAlteracoesAsync();
}
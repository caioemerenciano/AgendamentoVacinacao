using AgendamentoVacinacao.Repository.Context;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Repositories;

public class PacienteRepository : BaseRepository<Paciente>, IPacienteRepository
{
    public PacienteRepository(AgendamentoVacinacaoContext context) : base(context)
    {
    }

    public async Task<bool> ExistePacienteAsync(string nome, DateTime dataNascimento) =>
         await _dbset.Pacientes.AnyAsync(p => p.Nome!.ToLower() == nome.ToLower() && p.DataNascimento == dataNascimento);
    
    public async Task<Paciente?> ObterPorNomeEDataNascimentoAsync(string nome, DateTime dataNascimento) =>
        await _dbset.Pacientes.FirstOrDefaultAsync(p => p.Nome!.ToLower() == nome.ToLower() && p.DataNascimento == dataNascimento);

    public async Task AdicionarComIdForcadoAsync(Paciente paciente)
    {
        var sql = "SET IDENTITY_INSERT tb_paciente ON; " +
                  "INSERT INTO tb_paciente (id_paciente, dsc_nome, dat_nascimento, dat_criacao) " +
                  "VALUES ({0}, {1}, {2}, {3}); " +
                  "SET IDENTITY_INSERT tb_paciente OFF;";
        
        await _dbset.Database.ExecuteSqlRawAsync(sql, 
            paciente.Id, 
            paciente.Nome ?? string.Empty, 
            paciente.DataNascimento, 
            paciente.DataCriacao);
    }
}
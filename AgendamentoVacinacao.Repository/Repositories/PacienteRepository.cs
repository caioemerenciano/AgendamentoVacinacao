using AgendamentoVacinacao.Repository.Context;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly AgendamentoVacinacaoContext _context;

    public PacienteRepository(AgendamentoVacinacaoContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistePacienteAsync(string nome, DateTime dataNascimento) =>
         await _context.Pacientes.AnyAsync(p => p.Nome!.ToLower() == nome.ToLower() && p.DataNascimento == dataNascimento);
    
    public async Task<IEnumerable<Paciente>> ObterTodosAsync() =>
        await _context.Pacientes.AsNoTracking().ToListAsync();

    public async Task<Paciente?> ObterPorIdAsync(int id) =>
        await _context.Pacientes.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Paciente?> ObterPorNomeEDataNascimentoAsync(string nome, DateTime dataNascimento) =>
        await _context.Pacientes.FirstOrDefaultAsync(p => p.Nome!.ToLower() == nome.ToLower() && p.DataNascimento == dataNascimento);


    public async Task AdicionarAsync(Paciente paciente) =>
        await _context.Pacientes.AddAsync(paciente);

    public async Task AdicionarComIdForcadoAsync(Paciente paciente)
    {
        // Força o ID do paciente a ser o mesmo fornecido no objeto (que será o usuarioId)
        // Isso é necessário para que a regra de posse (PacienteId == UsuarioId) funcione.
        var sql = "SET IDENTITY_INSERT tb_paciente ON; " +
                  "INSERT INTO tb_paciente (id_paciente, dsc_nome, dat_nascimento, dat_criacao) " +
                  "VALUES ({0}, {1}, {2}, {3}); " +
                  "SET IDENTITY_INSERT tb_paciente OFF;";
        
        await _context.Database.ExecuteSqlRawAsync(sql, 
            paciente.Id, 
            paciente.Nome ?? string.Empty, 
            paciente.DataNascimento, 
            paciente.DataCriacao);
    }

    public async Task SalvarAlteracoesAsync() =>
        await _context.SaveChangesAsync();
}
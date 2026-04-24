using AgendamentoVacinacao.Entity.Entities;
using AgendamentoVacinacao.Repository.Interface.IRepositories;
using AgendamentoVacinacao.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace AgendamentoVacinacao.Repository.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AgendamentoVacinacaoContext _context;

    public UsuarioRepository(AgendamentoVacinacaoContext context)
    {
        _context = context;
    }
    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }
    public async Task AtualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}

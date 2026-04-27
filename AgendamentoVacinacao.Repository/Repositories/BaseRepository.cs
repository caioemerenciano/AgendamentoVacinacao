using AgendamentoVacinacao.Repository.Context;
using Microsoft.EntityFrameworkCore;
using AgendamentoVacinacao.Repository.Interface.IRepositories;

namespace AgendamentoVacinacao.Repository.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly AgendamentoVacinacaoContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(AgendamentoVacinacaoContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

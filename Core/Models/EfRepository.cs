using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using WebProject.Core.Interfaces;
using WebProject.Infastructure.Data;

namespace WebProject.Core.Models;

public class EfRepository<T> : IEntityRepository<T> where T : IdEntity
{
    private readonly AppDbContext _dbContext;
    public EfRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValueTask<T?> GetById(int id)
    {
        return _dbContext.Set<T>().FindAsync(id);
    }
    public async Task Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }
    public async Task Add(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<T> AsQueryable()
    {
        return _dbContext.Set<T>().AsQueryable();
    }
    public async Task Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<List<T>> ToList(int offset = 0, int limit = -1)
    {
        var entitySet = _dbContext.Set<T>();
        return limit > 0
            ? await entitySet.Skip(offset).Take(limit).ToListAsync()
            : await entitySet.Skip(offset).ToListAsync();
    }

    public async Task<int> Count()
    {
        return await _dbContext.Set<T>().CountAsync();
    }
}

using Microsoft.EntityFrameworkCore;
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
    
    public async Task Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
    public async Task<List<T>> ToList()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }
}

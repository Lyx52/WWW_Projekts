using Domain.Models;

namespace Domain.Interfaces;

public interface IEntityRepository<T> where T : IdEntity
{
    Task<List<T>> ToList(int offset = 0, int limit = -1);
    Task Remove(T entity);
    Task RemoveRange(ICollection<T> entities);
    Task Add(T entity);
    Task Update(T entity);
    ValueTask<T?> GetById(int id);
    IQueryable<T> AsQueryable();
    Task<int> Count();
}
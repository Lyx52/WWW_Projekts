using WebProject.Core.Models;

namespace WebProject.Core.Interfaces;

public interface IEntityRepository<T> where T : IdEntity
{
    Task<List<T>> ToList();
    Task Remove(T entity);
    Task Add(T entity);
    Task Update(T entity);
    ValueTask<T?> GetById(int id);
}
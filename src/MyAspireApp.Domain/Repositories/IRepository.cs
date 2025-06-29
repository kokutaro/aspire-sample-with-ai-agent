using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Domain.Repositories;

public interface IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

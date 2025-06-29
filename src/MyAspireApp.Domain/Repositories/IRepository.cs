using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Common;

namespace MyAspireApp.Domain.Repositories;

public interface IRepository<TEntity, in TId>
    where TEntity : Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    ValueTask<TEntity?> GetByIdAsync(TId id);
    Task<List<TEntity>> GetAllAsync();
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}

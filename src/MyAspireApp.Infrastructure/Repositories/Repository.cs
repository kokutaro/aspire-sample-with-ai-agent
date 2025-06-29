using Microsoft.EntityFrameworkCore;
using MyAspireApp.Domain.Common;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Repositories;

namespace MyAspireApp.Infrastructure.Repositories;

public class Repository<TEntity, TId>(ApplicationDbContext context) : IRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    protected readonly ApplicationDbContext Context = context ?? throw new ArgumentNullException(nameof(context));
    
    public ValueTask<TEntity?> GetByIdAsync(TId id) => context.Set<TEntity>().FindAsync(id);

    public Task<List<TEntity>> GetAllAsync() => context.Set<TEntity>().ToListAsync();

    public void Add(TEntity entity) => context.Set<TEntity>().Add(entity);

    public void Update(TEntity entity) => context.Set<TEntity>().Update(entity);

    public void Remove(TEntity entity) => context.Set<TEntity>().Remove(entity);
}
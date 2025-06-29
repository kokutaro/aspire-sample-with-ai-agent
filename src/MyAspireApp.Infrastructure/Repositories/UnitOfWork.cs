
using MyAspireApp.Domain.Repositories;

namespace MyAspireApp.Infrastructure.Repositories;

internal sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}

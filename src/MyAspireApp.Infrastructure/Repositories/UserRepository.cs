using Microsoft.EntityFrameworkCore;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.Repositories;

namespace MyAspireApp.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext context) : Repository<User, UserId>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        return !await Context.Set<User>().AnyAsync(u => u.Email.Value == email, cancellationToken);
    }
}

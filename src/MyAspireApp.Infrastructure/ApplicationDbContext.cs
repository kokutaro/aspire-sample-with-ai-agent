using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyAspireApp.Domain.Common;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Infrastructure.Converters;

namespace MyAspireApp.Infrastructure;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply StronglyTypedIdConverter to all StronglyTypedId properties
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.ClrType.IsGenericType ||
                    property.ClrType.GetGenericTypeDefinition() != typeof(StronglyTypedId<>) ||
                    property.ClrType.GetGenericArguments()[0] != typeof(Guid))
                {
                    continue;
                }

                var converterType = typeof(StronglyTypedIdConverter<>).MakeGenericType(property.ClrType);
                var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                property.SetValueConverter(converter);
            }
        }
    }
}

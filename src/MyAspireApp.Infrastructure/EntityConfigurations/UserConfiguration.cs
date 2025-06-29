using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyAspireApp.Domain.Entities;
using MyAspireApp.Domain.ValueObjects;

namespace MyAspireApp.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasConversion(v => v.Value, v => Email.Create(v));

        builder.HasIndex(u => u.Email).IsUnique();
    }
}

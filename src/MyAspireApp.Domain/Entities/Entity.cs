using MyAspireApp.Domain.Common;

namespace MyAspireApp.Domain.Entities;

public abstract class Entity<TId>
    where TId : StronglyTypedId<Guid>
{
    public TId Id { get; protected init; } = default!;

    // For ORM or serialization
    protected Entity(TId id)
    {
        Id = id;
    }

    private Entity() { }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right) => !(left == right);
}

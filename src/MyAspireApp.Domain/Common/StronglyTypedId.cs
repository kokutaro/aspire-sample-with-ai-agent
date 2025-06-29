namespace MyAspireApp.Domain.Common;

public abstract record StronglyTypedId<TId>(TId Value)
    : IComparable<StronglyTypedId<TId>>
    where TId : notnull
{
    public override string ToString() => Value.ToString()!;

    public int CompareTo(StronglyTypedId<TId>? other)
    {
        return other is null ? 1 : Comparer<TId>.Default.Compare(Value, other.Value);
    }
}

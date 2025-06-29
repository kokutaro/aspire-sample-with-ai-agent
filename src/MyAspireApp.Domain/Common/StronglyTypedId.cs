namespace MyAspireApp.Domain.Common;

public abstract record StronglyTypedId<TId>(TId Id)
    : IComparable<StronglyTypedId<TId>>
    where TId : notnull
{
    public override string ToString() => Id.ToString()!;

    public int CompareTo(StronglyTypedId<TId>? other)
    {
        return other is null ? 1 : Comparer<TId>.Default.Compare(Id, other.Id);
    }
}

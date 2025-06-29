namespace MyAspireApp.Domain.Common;

public abstract record StronglyTypedId<TValue> : IComparable<StronglyTypedId<TValue>>, IEquatable<StronglyTypedId<TValue>>
    where TValue : notnull
{
    public TValue Value { get; }

    protected StronglyTypedId(TValue value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString()!;

    public int CompareTo(StronglyTypedId<TValue>? other)
    {
        if (other is null) return 1;
        return Comparer<TValue>.Default.Compare(Value, other.Value);
    }
}

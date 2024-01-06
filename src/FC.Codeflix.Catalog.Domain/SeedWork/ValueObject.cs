namespace FC.Codeflix.Catalog.Domain.SeedWork;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract bool Equals(ValueObject? anOther);

    protected abstract int GetCustomHashCode();

    public override bool Equals(object? anObject)
    {
        if (ReferenceEquals(null, anObject)) return false;
        if (ReferenceEquals(this, anObject)) return true;

        return anObject.GetType() == GetType() && Equals((ValueObject)anObject);
    }

    public override int GetHashCode()
    {
        return GetCustomHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
        => left?.Equals(right) ?? false;

    public static bool operator !=(ValueObject? left, ValueObject? right)
        => !(left == right);
}
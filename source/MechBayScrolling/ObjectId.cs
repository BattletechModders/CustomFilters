using System;

namespace CustomFilters.MechBayScrolling;

internal readonly struct ObjectId : IEquatable<ObjectId>
{
    private readonly object? _id;
    internal ObjectId(object id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return $"{_id?.GetType().Name}@{GetHashCode()}";
    }

    public bool Equals(ObjectId other)
    {
        return Equals(_id, other._id);
    }

    public override bool Equals(object obj)
    {
        return obj is ObjectId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _id?.GetHashCode() ?? 0;
    }

    public static bool operator ==(ObjectId left, ObjectId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ObjectId left, ObjectId right)
    {
        return !left.Equals(right);
    }
}
namespace GMLib;

public interface IHandle
{
    internal ushort Index { get; }
}

public readonly struct Handle<T> : IEquatable<Handle<T>>, IHandle where T : class, IHandleable<T>
{
    ushort IHandle.Index => Index;
    private readonly ushort Index = 0;

    public Handle(ushort index)
    {
        Index = index;
    }

    public Handle(IHandle handle)
    {
        Index = handle.Index;
    }
    
    public static explicit operator WeakReference?(Handle<T> value)
    {
        return T.ResolveHandle(value);
    }
    
    public static explicit operator int(Handle<T> value)
    {
        return value.Index;
    }
    
    public override string ToString()
    {
        return $"Handle {typeof(T).Name} {Index}";
    }

    public bool Equals(Handle<T> other)
    {
        return Index == other.Index;
    }

    public override bool Equals(object? obj)
    {
        return obj is Handle<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public static bool operator ==(Handle<T> left, Handle<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Handle<T> left, Handle<T> right)
    {
        return !(left == right);
    }
}

namespace GMLib;

/// <summary>
/// This class works like the weak reference to deal with GM type that can be disposed when needed.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Handle<T> : IHandle, IEquatable<Handle<T>> where T : IHandleable<T>
{
    ushort IHandle.Index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    
    private readonly ushort Index = 0;
    public byte Prefix => (byte)(Index >> 8);
    public byte Id => (byte)(Index & 0x0F);

    public Handle(ushort index)
    {
        Index = index;
    }
    
    public Handle(IHandle handle)
    {
        Index = handle.Index;
    }

    public static implicit operator T?(Handle<T> value)
    {
        return T.ResolveHandle(value);
    }
    
    public static explicit operator int(Handle<T> value)
    {
        return value.Index;
    }
    
    public override string ToString()
    {
        // return Convert.ToString(Index, toBase: 2).PadLeft(16, '0');
        return $"Handle {Prefix.ToType().Name} {Id}";
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
}

public class Handle
{
}
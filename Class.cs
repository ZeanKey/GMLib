using System.Diagnostics.CodeAnalysis;

namespace GMLib;

/// <summary>
/// 
/// </summary>
public static partial class GML;

public readonly record struct UVs(int Left, int Top, int Right, int Bottom);

#region Handle
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleable<T> where T : IHandleable<T>
{
    public static abstract T? ResolveHandle(Handle<T> handle);
}

public interface IHandle
{
    internal ushort Index { get; set; }

    /// <summary>
    /// Compare the Handle ID with another Handle ID.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Only return true when the another object is Handle type and the intern 16-bit index is identical.</returns>
    public virtual bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is IHandle handle)
        {
            return handle.Index == Index;
        }
        return false;
    }
    
    /// <summary>
    /// Return the intern index value of the Handle ID.
    /// </summary>
    /// <returns>The intern 16-bit index value.</returns>
    public virtual int GetHashCode()
    {
        return Index;
    }

}

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

#endregion

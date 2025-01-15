global using Raylib_cs;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;

namespace GMLib;

/// <summary>
/// 
/// </summary>
public static partial class GML;

public static partial class GML
{
    public static void show_debug_message(params object[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg is null ? "undefined" : arg);
        }
    }
}

public readonly struct UVs
{
    public readonly int Left;
    public readonly int Top;
    public readonly int Right;
    public readonly int Bottom;

    public UVs(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}

#region Handle
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleable<T> where T : IHandleable<T>
{
    public abstract static T? ResolveHandle(Handle<T> handle);
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
public readonly struct Handle<T> : IHandle where T : IHandleable<T>
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
}

/*
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManagedHandle<T> where T : IHandleable<T>
{
    private readonly Handle<T> Handle;
    public bool IsValid { get; internal set; }

    internal ManagedHandle(Handle<T> handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Compare the Handle ID with another Handle ID.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Only return true when the another object is Handle type and the intern 16-bit index is identical.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ManagedHandle<T> managedHandle)
        {
            return Handle == managedHandle.Handle;
        }
        if (obj is Handle<T> handle)
        {
            return Handle == handle;
        }
        return false;
    }
    
    /// <summary>
    /// Return the intern index value of the Handle ID.
    /// </summary>
    /// <returns>The intern 16-bit index value.</returns>
    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    public static bool operator ==(ManagedHandle<T> left, ManagedHandle<T> right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(ManagedHandle<T> left, ManagedHandle<T> right)
    {
        return !(left == right);
    }

    public static implicit operator T?(ManagedHandle<T> value)
    {
        return (T?)value.Handle;
    }
}
*/
#endregion

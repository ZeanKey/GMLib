using System.Diagnostics.CodeAnalysis;

namespace GMLib;

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
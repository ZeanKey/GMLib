namespace GMLib.GM;

[Obsolete]
public static class Array<T>
{
    public static T[] Create(int size, T value = default!)
    {
        var ret = new T[size];
        for (var i = 0; i < size; i++)
        {
            ret[i] = value;
        }
        return ret;
    }

    public static void Copy(T[] dest, int destIndex, T[] src, int srcIndex, int length)
    {
        for (var i = 0; i < length; i++)
        {
            dest[destIndex + i] = src[srcIndex + i];
        }
    }

    public static bool Equals(T[] var1, T[] var2)
    {
        if (var1.Length != var2.Length) return false;
        return !var1.Where((t, i) => !t.Equals(var2[i])).Any();
    }

    public static T Get(T[] variable, int index)
    {
        return variable[index];
    }

    public static void Set(T[] variable, int index, T value)
    {
        variable[index] = value;
    }

    public static void Push(ref T[] variable, params T[] values)
    {
        var ret = new T[variable.Length + values.Length];
        variable.CopyTo(ret, 0);
        values.CopyTo(ret, variable.Length);
        variable = ret;
    }

    public static void Pop(ref T[] variable)
    {
        var ret = new T[variable.Length - 1];
        variable.CopyTo(ret, 0);
        variable = ret;
    }

    public static void Shift(ref T[] variable)
    {
        var ret = new T[variable.Length - 1];
        for (var i = 1; i < variable.Length; i++)
        {
            ret[i - 1] = variable[i];
        }
        variable = ret;
    }
    
    // TODO
}
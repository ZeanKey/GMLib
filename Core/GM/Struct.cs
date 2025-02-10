using System.Dynamic;

namespace GMLib.GM;

[Obsolete]
public static class Struct
{
    public static bool Exists(this IDictionary<string, object?> @struct, string name)
    {
        return @struct.ContainsKey(name);
    }
    
    public static object? Get(this IDictionary<string, object?> @struct, string name)
    {
        return @struct[name];
    }
    
    public static void Set(this IDictionary<string, object?> @struct, string name, object? val)
    {
        @struct[name] = val;
    }
    
    public static void Remove(IDictionary<string, object?> @struct, string name)
    {
        @struct.Remove(name);
    }
    
    public static string[] GetNames(this IDictionary<string, object?> @struct, string name)
    {
        return @struct.Keys.ToArray();
    }

    public static int NamesCount(this IDictionary<string, object?> @struct, string name)
    {
        return @struct.Count;
    }
}
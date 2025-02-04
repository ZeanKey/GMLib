namespace GMLib.GM.Variable;

public static class Global
{
    public static bool Exists(string name)
    {
        return ((IDictionary<string, object?>)GML.Global).ContainsKey(name);
    }

    public static object? Get(string name)
    {
        return ((IDictionary<string, object?>)GML.Global).TryGetValue(name, out var result) ? result : null;
    }

    public static void Set(string name, object? val)
    {
        ((IDictionary<string, object?>)GML.Global)[name] = val;
    }
}
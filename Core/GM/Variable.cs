using GMLib.GM.Extensions;

namespace GMLib.GM;

public class Variable
{
    public static class Instance
    {
        public static bool Exists(Handle<IObject> instanceId, string name)
            => instanceId.VariableExists(name);

        public static string[] NamesCount(Handle<IObject> instanceId)
            => instanceId.GetVariableNames();

        public static int GetCount(Handle<IObject> instanceId)
            => instanceId.GetVariableNamesCount();
    
        public static object? Get(Handle<IObject> instanceId, string name)
            => instanceId.GetVariable(name);
    
        public static void Set(Handle<IObject> instanceId, string name, object? val)
            => instanceId.SetVariable(name, val);
    }
    
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

    public static int GetHash(string name)
    {
        return name.GetHashCode();
    }

    public static TValue Clone<TValue>(TValue value, int depth = 128)
    {
        // TODO
        throw new NotImplementedException();
    }
}


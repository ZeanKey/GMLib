using System.Dynamic;

namespace GMLib;

partial class GML
{
    public static readonly ExpandoObject Global = new(); 
    
    public static bool VariableGlobalExists(string name) 
        => GM.Variable.Global.Exists(name);

    public static object? VariableGlobalGet(string name)
        => GM.Variable.Global.Get(name);

    public static void VariableGlobalSet(string name, object? val)
        => GM.Variable.Global.Set(name, val);
}
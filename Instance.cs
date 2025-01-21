using System.Dynamic;
using System.Reflection;

namespace GMLib;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ReservedInstanceField : Attribute
{
    public string Name { get; init; }
    public ReservedInstanceField(string name)
    {
        Name = name;
    }
}

/// <summary>
/// 
/// </summary>
public class Instance : DynamicObject, IObject
{
#region Reserved Fields
    [ReservedInstanceField("id")]
    public Handle<IObject>? Id { get; private set; }
    
    [ReservedInstanceField("self")]
    public Dictionary<string, object?>? Self { get; private set; }
    
    [ReservedInstanceField("object_index")]
    public Asset.Object ObjectIndex { get; private set; }
    
    [ReservedInstanceField("x")]
    public float X { get; set; }
    
    [ReservedInstanceField("y")]
    public float Y { get; set; }
    
    public Vector2 Position
    {
        get => new(X, Y);
        set { X = value.X; Y = value.Y; }
    }
#endregion

    /// <summary>
    /// 
    /// </summary>
    public static readonly Dictionary<string, MemberInfo> ReservedInstanceFields = [];
    
    static Instance()
    {
        var members = typeof(Instance).GetMembers()
            .Where(p => Attribute.IsDefined(p, typeof(ReservedInstanceField)));
       
        foreach (var member in members)
            ReservedInstanceFields.Add
            (
                (member.GetCustomAttribute(typeof(ReservedInstanceField))! as ReservedInstanceField)!.Name,
                member
            );
    }

    public Instance(Asset.Object objectAsset)
    {
        Self = [];
        ObjectIndex = objectAsset;
    }

#region Dynamic Object
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        return Self?.TryGetValue(binder.Name, out result) ?? throw new Exception();
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if ((IDictionary<string, object?>?)Self is { } resource)
        {
            resource[binder.Name] = value;
            return true;
        }
        return false;
    }

#endregion

#region IObject
    Handle<IObject>? IObject.Id { get => Id; set => Id = value; }

    bool IObject.EventPerform(GML.EventType type, params object?[] eventArgs)
    {
        return ObjectIndex.EventPerform(this, this, type, eventArgs);
    }

    bool IObject.InstanceDestroy()
    {
        Self = null;
        return GML.ObjectManager.InstanceDestroy(Id);
    }

    bool IObject.InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(Id);
    }

    public static implicit operator Handle<IObject>?(Instance instance)
    {
        return instance.Id;
    }
#endregion
}

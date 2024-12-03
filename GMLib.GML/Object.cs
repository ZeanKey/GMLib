using System.Dynamic;
using System.Reflection;

namespace GMLib;

public static partial class GML
{

}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ReservedInstanceField : Attribute
{
    public string Name { get; init; }
    public ReservedInstanceField(string name)
    {
        Name = name;
    }
}

public class Instance : DynamicObject, IObject
{
    Handle<IObject>? IObject.ID { get => ID; set => ID = value; }

    [ReservedInstanceField("id")]
    public Handle<IObject>? ID { get; private set; }
    [ReservedInstanceField("self")]
    public Dictionary<string, object?>? Self { get; private set; }
    [ReservedInstanceField("object_index")]
    public ObjectAsset ObjectIndex { get; private set; }

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

    public Instance(ObjectAsset objectAsset)
    {
        Self = [];
        ObjectIndex = objectAsset;
    }

    public static implicit operator Handle<IObject>?(Instance instance)
    {
        return instance.ID;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        return Self?.TryGetValue(binder.Name, out result) ?? throw new Exception();
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if ((IDictionary<string, object?>?)Self is IDictionary<string, object?> resource)
        {
            resource[binder.Name] = value;
            return true;
        }
        return false;
    }

    bool IObject.EventPerform(GML.Event type, params object?[] eventArgs)
    {
        return ObjectIndex.EventPerform(this, this, type, eventArgs);
    }

    bool IObject.InstanceDestroy()
    {
        Self = null;
        return GML.ObjectManager.InstanceDestroy(ID);
    }

    bool IObject.InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(ID);
    }
}

public class ObjectAsset : IHandleable<ObjectAsset>
{
    public delegate void Event(Instance sender, EventArguments args);
    
    public class EventArguments : EventArgs
    {
        public object ParamOther;
        public EventArguments(object other)
        {
            ParamOther = other;
        }
    }
    public class AlarmEventArguments : EventArguments
    {
        public int Index;
        public AlarmEventArguments(object other, int index) : base(other)
        {
            Index = index;
        }
    }

    public Dictionary<GML.Event, Event> EventCallbacks;
    public ObjectAsset? Parent;
    public HashSet<ObjectAsset> Children;

    public ObjectAsset()
    {
        EventCallbacks = [];
        Parent = null;
        Children = [];
    }
    public ObjectAsset(ObjectAsset? parent)
    {
        EventCallbacks = [];
        Parent = parent;
        if (parent is ObjectAsset parentObject)
        {
            parent.Children.Add(this);
        }
        Children = [];
    }
    public ObjectAsset(ObjectAsset? parent, IDictionary<GML.Event, Event> eventHandlers)
    {
        EventCallbacks = [];
        foreach (var eventname in eventHandlers.Keys)
        {
            EventCallbacks[eventname] = eventHandlers[eventname];
        }
        Parent = parent;
        if (parent is ObjectAsset parentObject)
        {
            parent.Children.Add(this);
        }
        Children = [];
    }

    public bool EventPerform(Instance paramSelf, Instance paramOther, GML.Event type, params object?[] eventArgs)
    {
        if (EventCallbacks.TryGetValue(type, out var eventHandler))
        {
            eventHandler.Invoke(paramSelf, new EventArguments(paramOther));
            return true;
        }
        return false;
    }

    public static ObjectAsset? ResolveHandle(Handle<ObjectAsset> handle)
    {
        throw new NotImplementedException();
    }
}
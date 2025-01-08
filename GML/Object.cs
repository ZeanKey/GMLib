using System.Dynamic;
using System.Reflection;

namespace GMLib;

public static partial class GML;

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
    /// <summary>
    /// 
    /// </summary>
    [ReservedInstanceField("id")]
    public Handle<IObject>? ID { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    [ReservedInstanceField("self")]
    public Dictionary<string, object?>? Self { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    [ReservedInstanceField("object_index")]
    public Asset.Object ObjectIndex { get; private set; }

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
        if ((IDictionary<string, object?>?)Self is IDictionary<string, object?> resource)
        {
            resource[binder.Name] = value;
            return true;
        }
        return false;
    }

#endregion

#region IObject
    Handle<IObject>? IObject.ID { get => ID; set => ID = value; }

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

    public static implicit operator Handle<IObject>?(Instance instance)
    {
        return instance.ID;
    }
#endregion
}

public partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
    [AssetClassOf(Type.Object)]
    public sealed class Object : IHandleable<Object>
    {
        public delegate void Event(Instance sender, EventArguments args);

#region Event Arguments
        /// <summary>
        /// 
        /// </summary>
        public class EventArguments : EventArgs
        {
            public object ParamOther;
            public EventArguments(object other)
            {
                ParamOther = other;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public class AlarmEventArguments : EventArguments
        {
            public int Index;
            public AlarmEventArguments(object other, int index) : base(other)
            {
                Index = index;
            }
        }
#endregion

        public Dictionary<GML.Event, Event> EventCallbacks;
        public Object? Parent;
        public HashSet<Object> Children;

        public Object()
        {
            EventCallbacks = [];
            Parent = null;
            Children = [];
        }
        public Object(Object? parent)
        {
            EventCallbacks = [];
            Parent = parent;
            parent?.Children.Add(this);
            Children = [];
        }
        public Object(Object? parent, IDictionary<GML.Event, Event> eventHandlers)
        {
            EventCallbacks = [];
            foreach (var eventname in eventHandlers.Keys)
            {
                EventCallbacks[eventname] = eventHandlers[eventname];
            }
            Parent = parent;
            parent?.Children.Add(this);
            Children = [];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramSelf"></param>
        /// <param name="paramOther"></param>
        /// <param name="type"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        public bool EventPerform(Instance paramSelf, Instance paramOther, GML.Event type, params object?[] eventArgs)
        {
            if (EventCallbacks.TryGetValue(type, out var eventHandler))
            {
                if (type == GML.Event.Alarm)
                {
                    eventHandler.Invoke(paramSelf, new AlarmEventArguments(paramOther, (int)eventArgs[0]!));
                    return true;
                }
                eventHandler.Invoke(paramSelf, new EventArguments(paramOther));
                return true;
            }
            return false;
        }

        public static Object? ResolveHandle(Handle<Object> handle)
        {
            throw new NotImplementedException();
        }
    }
}
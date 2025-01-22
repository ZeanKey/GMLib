
namespace GMLib;

public static partial class GML
{
    public static readonly ObjectManager Object = new();

    public static bool InstanceExists(Handle<IObject>? id)
    {
        return Object.InstanceExists(id);
    }
    
    public static void InstanceDestroy(Handle<IObject>? id)
    {
        Object.InstanceDestroy(id);
    }
    
    public static void EventPerform(ObjectEventType type, params object?[] args)
    {
        Object.EventPerform(type, args);
    }
}

public abstract class Object : IObject
{
    public Vector2 Position { get; set; }

    public Handle<IObject>? Id { get; private set; }
    Handle<IObject>? IObject.Id
    {
        get => Id;
        set => Id = value;
    }

    public abstract bool InstanceExists();
    
    public abstract bool InstanceDestroy();
    
    public abstract bool EventPerform(ObjectEventType type, params object?[] eventArgs);
}

public class LightObject : Object
{
    public delegate void EventCallback(LightObject sender, object?[] arguments);

    public readonly Dictionary<ObjectEventType, EventCallback> EventCallbacks;

    public LightObject()
    {
        EventCallbacks = [];
    }
    
    public LightObject(LightObject lightObject)
    {
        EventCallbacks = [];
        foreach (var @event in lightObject.EventCallbacks.Keys)
        {
            EventCallbacks[@event] = lightObject.EventCallbacks[@event];
        }
    }
    
    public LightObject(IDictionary<ObjectEventType, EventCallback> lightEventHandlers)
    {
        EventCallbacks = [];
        foreach (var @event in lightEventHandlers.Keys)
        {
            EventCallbacks[@event] = lightEventHandlers[@event];
        }
    }

    public LightObject AddEvent(ObjectEventType eventType, EventCallback callback)
    {
        EventCallbacks[eventType] = callback;
        return this;
    }
    
    public static implicit operator Handle<IObject>?(LightObject lightObject) => lightObject.Id;

    public override bool EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        if (!EventCallbacks.TryGetValue(type, out var eventHandler))
            return false;
        eventHandler.Invoke(this, eventArgs);
        return true;
    }

    public override bool InstanceDestroy()
    {
        return GML.Object.InstanceDestroy(Id);
    }

    public override bool InstanceExists()
    {
        return GML.Object.InstanceExists(Id);
    }
}

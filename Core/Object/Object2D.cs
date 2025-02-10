using GMLib.GM;

namespace GMLib;

partial class GML
{
    public static readonly ObjectManager Instance = new();
    
    public static bool InstanceExists(Handle<IObject>? id) =>
        id.Exists();
    
    public static void InstanceDestroy(Handle<IObject>? id) =>
        id.Destroy();
    
    public static void EventPerform(ObjectEventType type, params object?[] args)
    {
        Instance.EventPerform(type, args);
    }
}

public abstract class Object2D : IObject2D
{
    public int Depth { get; set; }
    
    public bool Visible { get; set; }
    
    public bool Persistent { get; set; }

    public Vector2 Position { get; set; }

    public Handle<IObject>? Id { get; private set; }
    Handle<IObject>? IObject.Id
    {
        get => Id;
        set => Id = value;
    }
    
    public abstract bool EventPerform(ObjectEventType type, params object?[] eventArgs);
}

public class LightObject2D : Object2D
{
    public delegate void EventCallback(LightObject2D sender, object?[] arguments);

    public readonly Dictionary<ObjectEventType, EventCallback> EventCallbacks;

    public LightObject2D()
    {
        EventCallbacks = [];
    }
    
    public LightObject2D(LightObject2D lightObject)
    {
        EventCallbacks = [];
        foreach (var @event in lightObject.EventCallbacks.Keys)
        {
            EventCallbacks[@event] = lightObject.EventCallbacks[@event];
        }
    }
    
    public LightObject2D(IDictionary<ObjectEventType, EventCallback> lightEventHandlers)
    {
        EventCallbacks = [];
        foreach (var @event in lightEventHandlers.Keys)
        {
            EventCallbacks[@event] = lightEventHandlers[@event];
        }
    }

    public LightObject2D AddEvent(ObjectEventType eventType, EventCallback callback)
    {
        EventCallbacks[eventType] = callback;
        return this;
    }
    
    public static implicit operator Handle<IObject>?(LightObject2D lightObject) => lightObject.Id;

    public override bool EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        if (!EventCallbacks.TryGetValue(type, out var eventHandler))
            return false;
        eventHandler.Invoke(this, eventArgs);
        return true;
    }
}

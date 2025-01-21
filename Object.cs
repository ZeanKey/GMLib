
namespace GMLib;

public static partial class GML
{
    public enum EventType
    {
        Create,
        Step,
        Draw,
        Destroy,
        CleanUp,
        Alarm
    }
    
    public static readonly ObjectManager ObjectManager = new();

    public static bool InstanceExists(Handle<IObject>? id)
    {
        return ObjectManager.InstanceExists(id);
    }
    
    public static void InstanceDestroy(Handle<IObject>? id)
    {
        ObjectManager.InstanceDestroy(id);
    }
    
    public static void EventPerform(EventType type, params object?[] args)
    {
        ObjectManager.EventPerform(type, args);
    }
}

/// <summary>
/// The interface of template game object type.<br/>
/// Literally, any class inherit from this interface can be managed be ObjectManager.
/// </summary>
public interface IObject : IHandleable<IObject>
{
    static IObject? IHandleable<IObject>.ResolveHandle(Handle<IObject> handle)
    {
        GML.ObjectManager.TryGetInstance(handle, out var value);
        return value;
    }

    public Vector2 Position { get; set; }
    
    public Handle<IObject>? Id { get; internal set; }
    
    public bool InstanceExists();
    
    public bool InstanceDestroy();
    
    public bool EventPerform(GML.EventType type, params object?[] eventArgs);
    
    public class TimerAlarm
    {
        private int Timer;
        internal Action<IObject> Callback { get; private set; }
        
        public TimerAlarm(int time, Action<IObject> callback)
        {
            Timer = time;
            Callback = callback;
        }
        
        public TimerAlarm(int timeBegin, int timeEnd, Action<IObject> callback)
        {
            Timer = timeEnd - timeBegin;
            Callback = callback;
        }

        internal void Update(out bool isTriggered)
        {
            Timer --;
            if (Timer <= 0)
            {
                isTriggered = true;
                return;
            }
            isTriggered = false;
        }
        
        internal void Trigger(IObject caller)
        {
            Callback?.Invoke(caller);
        }
    }
    
    public class TimerAlarmManager
    {
        private readonly IObject Owner;
        private readonly List<TimerAlarm> Alarms = [];
        
        internal TimerAlarmManager(IObject owner)
        {
            Owner = owner;
        }

        #region Add
        public TimerAlarmManager Add(TimerAlarm alarm)
        {
            Alarms.Add(alarm);
            return this;
        }
        
        public TimerAlarmManager Add(int time, Action<IObject> callback)
        {
            Alarms.Add(new TimerAlarm(time, callback));
            return this;
        }
        
        public TimerAlarmManager Add(int timeBegin, int timeEnd, Action<IObject> callback)
        {
            Alarms.Add(new TimerAlarm(timeBegin, timeEnd, callback));
            return this;
        }
        #endregion

        internal virtual void Update()
        {
            for (var i = 0; i < Alarms.Count; i++)
            {
                var alarm = Alarms[i];
                alarm.Update(out var isTriggered);
                
                if (!isTriggered)
                    continue;
                
                alarm.Trigger(Owner);
                Alarms.RemoveAt(i);
                i--;
            }
        }
        
        public static TimerAlarmManager operator+ (TimerAlarmManager alarmManager, TimerAlarm alarm)
        {
            return alarmManager.Add(alarm);
        }
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
    
    public abstract bool EventPerform(GML.EventType type, params object?[] eventArgs);
}

/// <summary>
/// The base object of the template when construct user's own game object class.<br/>
/// </summary>
public class GameObject: Object
{
    /// <summary>
    /// 
    /// </summary>
    public virtual void Create()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Step()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Draw()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void DrawGUI()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Destroy()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void CleanUp()
    {
        
    }
    
    public override bool InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(Id);
    }
    
    public override bool InstanceDestroy()
    {
        return GML.ObjectManager.InstanceDestroy(Id);
    }

    public override bool EventPerform(GML.EventType type, params object?[] eventArgs)
    {
        switch (type)
        {
            case GML.EventType.Create:
                Create();
                return true;
            case GML.EventType.Step:
                Step();
                return true;
            case GML.EventType.Draw:
                Draw();
                return true;
            case GML.EventType.Destroy:
                Destroy();
                return true;
            case GML.EventType.CleanUp:
                CleanUp();
                return true;
            default:
                return false;
        }
    }
    
    public static implicit operator Handle<IObject>?(GameObject gameObject) => gameObject.Id;
}

/// <summary>
/// The game object manager class.<br/>
/// This class provides the function of invoke game objects' event under game main loop.
/// It's possible to build users' own object manager with this class but the involving function are currenly marked as private or internal now.
/// </summary>
public class ObjectManager
{
    private ushort _index;
    protected Dictionary<Handle<IObject>, IObject> MergedCollection
    {
        get
        {
            var collection = _collection
                .ToDictionary(objectPair => objectPair.Key, objectPair => objectPair.Value);
            foreach (var objectPair in SubManagers.SelectMany(subManager => subManager._collection))
                collection.Add(objectPair.Key, objectPair.Value);
            return collection;
        }
    }

    private readonly Dictionary<Handle<IObject>, IObject> _collection = [];
    
    internal List<ObjectManager> SubManagers { get; } = [];
    
    private Handle<IObject> NextInstanceId() => new(_index ++);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetInstance(Handle<IObject>? id, out IObject? value)
    {
        if (id is { } vid)
        {
            if (_collection.TryGetValue(vid, out value))
                return true;
            foreach (var submanager in SubManagers)
                if (submanager.TryGetInstance(vid, out value)) return true;
            return false;
        }
        value = null;
        return false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="eventArgs"></param>
    public void EventPerform(GML.EventType type, params object?[] eventArgs)
    {
        foreach (var id in _collection.Keys)
        {
            if (TryGetInstance(id, out var gMObject) && gMObject is not null)
                gMObject.EventPerform(type, eventArgs);
        }
        foreach (var submanager in SubManagers)
            submanager.EventPerform(type, eventArgs);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gMObject"></param>
    /// <returns></returns>
    public Handle<IObject>? AddObject(IObject gMObject)
    {
        if (_collection.ContainsValue(gMObject))
        {
            return null;
        }

        var instanceId = NextInstanceId();
        gMObject.Id = instanceId;
        _collection.Add(instanceId, gMObject);

        return instanceId;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveObject(Handle<IObject>? id)
    {
        if (id is not { } vid)
            return false;
        
        if (_collection.Remove(vid))
            return true;
        
        foreach (var subManager in SubManagers)
            if (subManager.RemoveObject(id))
                return true;
        
        return false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool InstanceExists(Handle<IObject>? id)
    {
        if (id is not { } vid)
            return false;
        
        if (_collection.ContainsKey(vid))
            return true;
        
        foreach (var subManager in SubManagers)
            if (subManager.InstanceExists(id))
                return true;
        
        return false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool InstanceDestroy(Handle<IObject>? id)
    {
        return RemoveObject(id);
    }
}


/// <summary>
/// The object can be defined more flexible, mainly for temporary test case.
/// </summary>
public class LightObject : Object
{
    public delegate void EventCallback(LightObject sender, object?[] arguments);


    public readonly Dictionary<GML.EventType, EventCallback> EventCallbacks;

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
    
    public LightObject(IDictionary<GML.EventType, EventCallback> lightEventHandlers)
    {
        EventCallbacks = [];
        foreach (var @event in lightEventHandlers.Keys)
        {
            EventCallbacks[@event] = lightEventHandlers[@event];
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public LightObject AddEvent(GML.EventType eventType, EventCallback callback)
    {
        EventCallbacks[eventType] = callback;
        return this;
    }
    
    public static implicit operator Handle<IObject>?(LightObject lightObject) => lightObject.Id;

    public override bool EventPerform(GML.EventType type, params object?[] eventArgs)
    {
        if (!EventCallbacks.TryGetValue(type, out var eventHandler))
            return false;
        eventHandler.Invoke(this, eventArgs);
        return true;
    }

    public override bool InstanceDestroy()
    {
        return GML.ObjectManager.InstanceDestroy(Id);
    }

    public override bool InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(Id);
    }
}

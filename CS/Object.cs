namespace GMLib;

public static partial class GML
{
    /// <summary>
    /// 
    /// </summary>
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool InstanceExists(Handle<IObject>? id)
    {
        return ObjectManager.InstanceExists(id);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public static void InstanceDestroy(Handle<IObject>? id)
    {
        ObjectManager.InstanceDestroy(id);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTriggered"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
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
        
        /// <summary>
        /// 
        /// </summary>
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
    
    public Handle<IObject>? ID { get; internal set; }
    
    public bool InstanceExists();
    
    public bool InstanceDestroy();
    
    public abstract bool EventPerform(GML.EventType type, params object?[] eventArgs);
}

/// <summary>
/// The base object of the template when construct user's own game object class.<br/>
/// </summary>
public abstract class Object: IObject
{
    Handle<IObject>? IObject.ID { get => ID; set => ID = value; }
    
    public Handle<IObject>? ID;
    
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(ID);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool InstanceDestroy()
    {
        return GML.ObjectManager.InstanceDestroy(ID);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="eventArgs"></param>
    /// <returns></returns>
    public bool EventPerform(GML.EventType type, params object?[] eventArgs)
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
    
    public static implicit operator Handle<IObject>?(Object gameObject) => gameObject.ID;
}

#region Object ID * Abandoned
/*
/// <summary>
/// The id type for the game object.<br/>
/// It's not recommand to direcly using this id to control the object refering to although it's supported due to GameMaker syntex.
/// This id should only be used when comparing with two object under GM code and managing object with GM.ObjectManager.
/// </summary>
public readonly struct ObjectID
{
    private readonly ushort _index = 0;

    public ObjectID(ushort index)
    {
        _index = index;
    }

    /// <summary>
    /// Compare the GMObject ID with another GMObject ID.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Only return true when the another object is GMObjectID type and the intern 16-bit index is identical.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is ObjectID instanceId)
        {
            return instanceId._index == _index;
        }
        return false;
    }
    
    /// <summary>
    /// Return the intern index value of the GMObject ID.
    /// </summary>
    /// <returns>The intern 16-bit index value.</returns>
    public override int GetHashCode()
    {
        return _index;
    }

    public static bool operator ==(ObjectID left, ObjectID right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(ObjectID left, ObjectID right)
    {
        return !(left == right);
    }
}
*/
#endregion

/// <summary>
/// The game object manager class.<br/>
/// This class provides the function of invoke game objects' event under game main loop.
/// It's possible to build users' own object manager with this class but the involving function are currenly marked as private or internal now.
/// </summary>
public class ObjectManager
{
    private ushort _currentInstanceIndex;
    private readonly Dictionary<Handle<IObject>, IObject> _instanceCollection = [];

    private Handle<IObject> NextInstanceId() => new(_currentInstanceIndex ++);
    
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
            return _instanceCollection.TryGetValue(vid, out value);
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
        foreach (Handle<IObject> id in _instanceCollection.Keys)
        {
            if (TryGetInstance(id, out IObject? gMObject) && gMObject is not null)
            {
                gMObject.EventPerform(type, eventArgs);
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gMObject"></param>
    /// <returns></returns>
    public Handle<IObject>? AddObject(IObject gMObject)
    {
        if (_instanceCollection.ContainsValue(gMObject))
        {
            return null;
        }

        Handle<IObject> instanceId = NextInstanceId();
        gMObject.ID = instanceId;
        _instanceCollection.Add(instanceId, gMObject);

        return instanceId;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool RemoveObject(Handle<IObject>? id)
    {
        if (id is { } vid)
        {
            return _instanceCollection.Remove(vid);
        }
        return false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool InstanceExists(Handle<IObject>? id)
    {
        if (id is { } vid)
        {
            return _instanceCollection.ContainsKey(vid);
        }
        return false;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool InstanceDestroy(Handle<IObject>? id)
    {
        if (id is { } vid)
        {
            return _instanceCollection.Remove(vid);
        }
        return false;
    }
}


/// <summary>
/// The object can be defined more flexible, mainly for temporary test case.
/// </summary>
public class LightObject : IObject
{
    public delegate void EventCallback(LightObject sender, object?[] arguments);

    public Handle<IObject>? ID;
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
    
    #region IObject
    Handle<IObject>? IObject.ID { get => ID; set => ID = value; }

    public static implicit operator Handle<IObject>?(LightObject lightObject) => lightObject.ID;

    bool IObject.EventPerform(GML.EventType type, params object?[] eventArgs)
    {
        if (!EventCallbacks.TryGetValue(type, out var eventHandler))
            return false;
        eventHandler.Invoke(this, eventArgs);
        return true;
    }

    bool IObject.InstanceDestroy()
    {
        return GML.ObjectManager.InstanceDestroy(ID);
    }

    bool IObject.InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(ID);
    }

    #endregion
}

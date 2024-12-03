using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;

namespace GMLib;

public static partial class GML
{
    public enum Event
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
    public static void EventPerform(Event type, params object?[] args)
    {
        ObjectManager.EventPerform(type, args);
    }
}

public interface IHandleable<T> where T : IHandleable<T>
{
    public abstract static T? ResolveHandle(Handle<T> handle);
}

/// <summary>
/// This class works like the weak reference to deal with GM type that can be disposed when needed.
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct Handle<T> where T : IHandleable<T>
{
    private readonly ushort _index = 0;

    public Handle(ushort index)
    {
        _index = index;
    }

    /// <summary>
    /// Compare the Handle ID with another Handle ID.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Only return true when the another object is Handle type and the intern 16-bit index is identical.</returns>
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Handle<T> handle)
        {
            return handle._index == _index;
        }
        return false;
    }
    
    /// <summary>
    /// Return the intern index value of the Handle ID.
    /// </summary>
    /// <returns>The intern 16-bit index value.</returns>
    public override int GetHashCode()
    {
        return _index;
    }

    public static bool operator ==(Handle<T> left, Handle<T> right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Handle<T> left, Handle<T> right)
    {
        return !(left == right);
    }

    public static implicit operator T?(Handle<T> value)
    {
        if (typeof(T).GetMethod("ResolveHandle") is MethodInfo method && MethodInvoker.Create(method).Invoke(null, value) is T obj)
        {
            return obj;
        }
        return default;
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
        GML.ObjectManager.TryGetInstance(handle, out IObject? value);
        return value;
    }
    public class TimerAlarm
    {
        private int timer;
        internal Action<IObject> Callback { get; private set; }
        public TimerAlarm(int time, Action<IObject> callback)
        {
            timer = time;
            Callback = callback;
        }
        public TimerAlarm(int timeBegin, int timeEnd, Action<IObject> callback)
        {
            timer = timeEnd - timeBegin;
            Callback = callback;
        }
        internal void Update(out bool isTriggered)
        {
            timer --;
            if (timer <= 0)
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
        private readonly IObject owner;
        private readonly List<TimerAlarm> alarms = [];
        internal TimerAlarmManager(IObject owner)
        {
            this.owner = owner;
        }
        
        public static TimerAlarmManager operator+ (TimerAlarmManager alarmManager, TimerAlarm alarm)
        {
            return alarmManager.Add(alarm);
        }
        public TimerAlarmManager Add(TimerAlarm alarm)
        {
            alarms.Add(alarm);
            return this;
        }
        public TimerAlarmManager Add(int time, Action<IObject> callback)
        {
            alarms.Add(new TimerAlarm(time, callback));
            return this;
        }
        public TimerAlarmManager Add(int timeBegin, int timeEnd, Action<IObject> callback)
        {
            alarms.Add(new TimerAlarm(timeBegin, timeEnd, callback));
            return this;
        }
        internal virtual void Update()
        {
            for (int i = 0; i < alarms.Count; i++)
            {
                TimerAlarm alarm = alarms[i];
                alarm.Update(out bool isTriggered);
                if (isTriggered)
                    alarm.Trigger(owner);
                    alarms.RemoveAt(i);
            }
        }
    }
    public Handle<IObject>? ID { get; internal set; }
    public bool InstanceExists();
    public bool InstanceDestroy();
    public abstract bool EventPerform(GML.Event type, params object?[] eventArgs);
}

/// <summary>
/// The base object of the template when construct user's own game object class.<br/>
/// </summary>
public abstract class Object: IObject
{
    Handle<IObject>? IObject.ID { get => ID; set => ID = value; }
    public Handle<IObject>? ID;

    public static implicit operator Handle<IObject>?(Object gMObject)
    {
        return gMObject.ID;
    }

    public virtual void Create() {}
    public virtual void Step() {}
    public virtual void Draw() {}
    public virtual void DrawGUI() {}
    public virtual void Destroy() {}
    public virtual void CleanUp() {}

    public bool InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(ID);
    }

    public bool InstanceDestroy()
    {
        return GML.ObjectManager.InstanceDestroy(ID);
    }

    public bool EventPerform(GML.Event type, params object?[] eventArgs)
    {
        switch (type)
        {
            case GML.Event.Create:
            Create();
            return true;
            case GML.Event.Step:
            Step();
            return true;
            case GML.Event.Draw:
            Draw();
            return true;
            case GML.Event.Destroy:
            Destroy();
            return true;
            case GML.Event.CleanUp:
            CleanUp();
            return true;
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

    Handle<IObject>? IObject.ID { get => ID; set => ID = value; }
    public Handle<IObject>? ID;
    public Dictionary<GML.Event, EventCallback> EventCallbacks;

    public LightObject()
    {
        EventCallbacks = [];
    }
    public LightObject(LightObject lightObject)
    {
        EventCallbacks = [];
        foreach (var eventname in lightObject.EventCallbacks.Keys)
        {
            EventCallbacks[eventname] = lightObject.EventCallbacks[eventname];
        }
    }
    public LightObject(IDictionary<GML.Event, EventCallback> lightEventHandlers)
    {
        EventCallbacks = [];
        foreach (var eventname in lightEventHandlers.Keys)
        {
            EventCallbacks[eventname] = lightEventHandlers[eventname];
        }
    }

    public static implicit operator Handle<IObject>?(LightObject gMObject)
    {
        return gMObject.ID;
    }

    bool IObject.EventPerform(GML.Event type, params object?[] eventArgs)
    {
        if (EventCallbacks.TryGetValue(type, out var eventHandler))
        {
            eventHandler.Invoke(this, eventArgs);
            return true;
        }
        return false;
    }

    bool IObject.InstanceDestroy()
    {
        return GML.ObjectManager.InstanceDestroy(ID);
    }

    bool IObject.InstanceExists()
    {
        return GML.ObjectManager.InstanceExists(ID);
    }
}

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

/// <summary>
/// The game object manager class.<br/>
/// This class provides the function of invoke game objects' event under game main loop.
/// It's possible to build users' own object manager with this class but the involving function are currenly marked as private or internal now.
/// </summary>
public class ObjectManager
{
    private ushort _currentInstanceIndex;
    private readonly Dictionary<Handle<IObject>, IObject> _instanceCollection;
    
    public ObjectManager()
    {
        _currentInstanceIndex = 0;
        _instanceCollection = [];
    }
    
    private Handle<IObject> NextInstanceId()
    {
        return new Handle<IObject>(_currentInstanceIndex ++);
    }
    
    public bool TryGetInstance(Handle<IObject>? id, out IObject? value)
    {
        if (id is Handle<IObject> vid)
        {
            return _instanceCollection.TryGetValue(vid, out value);
        }
        value = null;
        return false;
    }
    public void EventPerform(GML.Event type, params object?[] eventArgs)
    {
        foreach (Handle<IObject> id in _instanceCollection.Keys)
        {
            if (TryGetInstance(id, out IObject? gMObject) && gMObject is not null)
            {
                gMObject.EventPerform(type, eventArgs);
            }
        }
    }

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
    public bool RemoveObject(Handle<IObject>? id)
    {
        if (id is Handle<IObject> vid)
        {
            return _instanceCollection.Remove(vid);
        }
        return false;
    }
    
    public bool InstanceExists(Handle<IObject>? id)
    {
        if (id is Handle<IObject> vid)
        {
            return _instanceCollection.ContainsKey(vid);
        }
        return false;
    }
    public bool InstanceDestroy(Handle<IObject>? id)
    {
        if (id is Handle<IObject> vid)
        {
            return _instanceCollection.Remove(vid);
        }
        return false;
    }
}

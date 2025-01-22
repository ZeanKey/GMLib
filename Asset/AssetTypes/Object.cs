namespace GMLib;

public static partial class GML;

public partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
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

        public Dictionary<ObjectEventType, Event> EventCallbacks;
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
        public Object(Object? parent, IDictionary<ObjectEventType, Event> eventHandlers)
        {
            EventCallbacks = [];
            foreach (var eventName in eventHandlers.Keys)
            {
                EventCallbacks[eventName] = eventHandlers[eventName];
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
        public bool EventPerform(Instance paramSelf, Instance paramOther, ObjectEventType type, params object?[] eventArgs)
        {
            if (EventCallbacks.TryGetValue(type, out var eventHandler))
            {
                if (type == ObjectEventType.Alarm)
                {
                    eventHandler.Invoke(paramSelf, new AlarmEventArguments(paramOther, (int)eventArgs[0]!));
                    return true;
                }
                eventHandler.Invoke(paramSelf, new EventArguments(paramOther));
                return true;
            }
            return false;
        }

        public static WeakReference ResolveHandle(Handle<Object> handle)
        {
            throw new NotImplementedException();
        }
    }
}
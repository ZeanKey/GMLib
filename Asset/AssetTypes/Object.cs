namespace GMLib;

public static partial class GML;

public partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
    [RegisterAssetType(AssetType.Object)]
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

        public Dictionary<GML.EventType, Event> EventCallbacks;
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
        public Object(Object? parent, IDictionary<GML.EventType, Event> eventHandlers)
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
        public bool EventPerform(Instance paramSelf, Instance paramOther, GML.EventType type, params object?[] eventArgs)
        {
            if (EventCallbacks.TryGetValue(type, out var eventHandler))
            {
                if (type == GML.EventType.Alarm)
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
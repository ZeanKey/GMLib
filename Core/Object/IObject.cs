namespace GMLib;

public interface IObject : IHandleable<IObject>
{
    static WeakReference? IHandleable<IObject>.ResolveHandle(Handle<IObject> handle)
    {
        GML.Object.TryFindObject(handle, out var value);
        return value;
    }

    public Handle<IObject>? Id { get; internal set; }
    
    public int Depth { get; set; }
    
    public bool Visible { get; set; }
    
    public bool Persistent { get; set; }
    
    public bool Exists();
    
    public bool Destroy();
    
    public bool EventPerform(ObjectEventType type, params object?[] eventArgs);
    
    public class TimerAlarm
    {
        public int Timer;
        private Action<IObject> Callback { get; set; }
        
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

        internal void Update(IObject caller, out bool isTriggered)
        {
            if (Timer <= 0)
            {
                isTriggered = false;
                return;
            }
            Timer --;
            if (Timer <= 0)
            {
                isTriggered = true;
                Trigger(caller);
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
        private readonly List<TimerAlarm> Alarms = [];

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

        internal virtual void Update(IObject caller)
        {
            for (var i = 0; i < Alarms.Count; i++)
            {
                var alarm = Alarms[i];
                alarm.Update(caller, out var isTriggered);
                
                if (!isTriggered)
                    continue;
                
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
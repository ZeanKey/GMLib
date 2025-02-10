using GMLib.GM;

namespace GMLib;

public interface IGameObject : IObject2D
{
    public bool Solid { get; set; }
    
    public Asset.Room.Layer<Asset.Room.Element.Instance> Layer { get; set; }
    
    public IndexedTimerAlarmManager Alarm { get; }
    
    public float Friction { get; set; }
    
    public Vector2 Velocity { get; set; }
    
    public Vector2 Gravity { get; set; }
    
    public Vector2 PositionStart { get; }
    
    public Vector2 PositionPrevious { get; }
    
    public Handle<Asset.Sprite> SpriteIndex { get; set; }

    public Vector2 SpriteSize => SpriteIndex.GetSize() * ImageScale;
    
    public Vector2 SpriteOffset => SpriteIndex.GetOffset() * ImageScale;
    
    public Color ImageBlend { get; set; }
    
    public float ImageAngle { get; set; }
    
    public int ImageIndex { get; set; }

    public int ImageNumber => SpriteIndex.GetNumber();
    
    public float ImageSpeed { get; set; }
    
    public Vector2 ImageScale { get; set; }
    
    public Handle<Asset.Sprite> MaskIndex { get; set; }

    public Rectangle BBox { get; set; }

    public class IndexedTimerAlarmManager : TimerAlarmManager
    {
        private readonly TimerAlarm[] Alarms = new TimerAlarm[12];

        public int this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        public int Get(int index)
        {
            return Alarms[index].Timer;
        }
        
        public void Set(int index, int value)
        {
            Alarms[index].Timer = value;
        }
        
        internal void SetCallback(int index, Action<IObject> callback)
        {
            Alarms[index] = new TimerAlarm(0, callback);
        }

        internal override void Update(IObject caller)
        {
            foreach (var alarm in Alarms)
                alarm.Update(caller, out _);
        }
    }
}
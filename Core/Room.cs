using System.Numerics;

namespace GMLib;

public partial class GML
{
    public static Room Room;
    public static int RoomWidth => (int)Room.Size.X;
    public static int RoomHeight => (int)Room.Size.Y;
}

public class Room
{
    public Vector2 Size { get; init; }
    
    private ObjectManager ObjectManager = new ObjectManager();

    public Room(Asset.Room asset)
    {
        Size = asset.Size;
    }
        
    public void Init()
    {
        GML.Object.SubManagers.Add(ObjectManager);
    }

    public void Uninit()
    {
        GML.Object.SubManagers.Remove(ObjectManager);
    }
}

public interface ICamera
{
    public Rectangle View { get; set; }
    
    public Matrix4x4 ViewMatrix { get; set; }
    
    public Matrix4x4 ProjectionMatrix { get; set; }

    public void Update();
    
    public void Begin();
    
    public void End();
}

public class Camera : ICamera
{
    public float Angle { get; set; }
    
    public Vector2 Speed { get; set; }
    
    public Vector2 Border { get; set; }
    
    public Rectangle View { get; set; }
    
    public Matrix4x4 ViewMatrix { get; set; }
    
    public Matrix4x4 ProjectionMatrix { get; set; }

    public IGameObject? Target { get; set; }

    public event Action<Camera>? UpdateEvent;
    
    public event Action<Camera>? BeginEvent;
    
    public event Action<Camera>? EndEvent;
    
    public void Update()
    {
        UpdateEvent?.Invoke(this);
    }

    public void Begin()
    {
        BeginEvent?.Invoke(this);
    }

    public void End()
    {
        EndEvent?.Invoke(this);
    }
}

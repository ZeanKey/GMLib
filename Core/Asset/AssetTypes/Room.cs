namespace GMLib;

partial class GML
{
    public static RoomManager Room => (RoomManager)Asset.GetManager<Asset.Room>();
}

public class RoomManager : AssetManager<Asset.Room>
{
    public readonly List<Handle<Asset.Room>> Order = []; 
    
    public int FindRoomIndex(string roomName)
    {
        return Order.IndexOf(Assets[roomName]);
    }

    public int FindRoomIndex(Asset.Room room)
    {
        return Order.IndexOf(Contents.FirstOrDefault(pair => pair.Value == room).Key);
    }
    
    public Handle<Asset.Room> Register(int index, string name, Asset.Room asset)
    {
        var handle = Create(asset);
        Assets[name] = handle;
        Order.Insert(index, handle);
        return handle;
    }
}

partial class Asset
{
    public class Room : Asset, IHandleable<Room>
    {
        public Vector2 Size;
        public bool Persistent;
        
        public bool ClearDisplayBuffer;
        public bool ClearViewportBackground;
        public bool EnableViews;
        public bool InheritViewSettings;

        public Color Color;

        public Element.Instance[] Instances;
        public Layer[] Layers;
        public View[] Views;

        public static WeakReference? ResolveHandle(Handle<Room> handle)
        {
            return GML.Room.TryFindAsset(handle, out var sprite) ? sprite : null;
        }
        
        public abstract class Element
        {
            public class Background : Element
            {
            }
            
            public class Instance : Element
            {
                public string Name;
                public Color Color;
                public bool Frozen;
                public Action<IObject>? CreationCode;
                public int ImageIndex;
                public float ImageSpeed;
                public float Rotation;
                public Vector2 Scale;
                public Vector2 Position;
            }
            
            public class Sprite : Element
            {
            }
            
            public class Tilemap : Element
            {
                
            }

            public class ParticleSystem : Element
            {
                
            }
            
            public class Tile : Element
            {
                
            }
            
            public class Sequence : Element
            {
                
            }
            
            public class Undefined : Element
            {
                
            }
        }
        
        public abstract class Layer
        {
            public string Name { get; set; }
            public bool Visible { get; set; }
            public int Depth { get; set; }
        }
        
        public class Layer<T> : Layer where T : Element
        {
            public T[] Elements { get; set; }
        }
        
        public class View
        {
            public bool Visible;
            public Rectangle Port;
            public Camera Camera;
        }
        
        public class Camera
        {
            public Vector2 Speed;
            public Vector2 Border;
            public Rectangle Rec;
            public string Target;
        }

        static Room()
        {
            AssetManager.Managers[typeof(Room)] = new RoomManager();
        }
    }
}
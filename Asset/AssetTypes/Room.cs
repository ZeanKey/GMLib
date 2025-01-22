using System.Numerics;

namespace GMLib;

public partial class Asset
{
    public class Room : Asset, IHandleable<Room>
    {
        public Vector2 Size { get; set; }

        public Room()
        {

        }

        public static WeakReference ResolveHandle(Handle<Room> handle)
        {
            throw new NotImplementedException();
        }
    }
}
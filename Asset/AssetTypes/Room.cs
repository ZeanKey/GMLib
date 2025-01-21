using System.Numerics;

namespace GMLib;

public partial class Asset
{
    [RegisterAssetType(AssetType.Room)]
    public class Room : Asset, IHandleable<Room>
    {
        public Vector2 Size { get; set; }
        
        public Room(string name) : base(name) 
        {
            
        }

        public static Room? ResolveHandle(Handle<Room> handle)
        {
            throw new NotImplementedException();
        }
    }
}
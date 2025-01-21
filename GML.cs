namespace GMLib;

public static partial class GML
{
    public static void Init()
    {
        Room = new Room(new Asset.Room("roomTest") {
            Size = new Vector2(640, 480)
        });
    }
}
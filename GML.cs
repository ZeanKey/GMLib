namespace GMLib;

public static partial class GML
{
    public static void Init()
    {
        Room = new Room(new Asset.Room() {
            Size = new Vector2(640, 480)
        });
    }
}
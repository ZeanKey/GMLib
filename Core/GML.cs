namespace GMLib;

public static partial class GML
{
    public static void Init()
    {
        Instance.SubManagers.Add(CurrentRoom);
        ApplicationSurface = Raylib.LoadRenderTexture(640, 480);
        
        var room = new Asset.Room() {
            Size = new Vector2(6400, 480),
            EnableViews = true,
            ClearViewportBackground = true,
            Color = Color.Black,
            Views = [new Asset.Room.View()
            {
                Port = new Rectangle(Vector2.Zero, 320, 240),
                Visible = true,
                Camera = new Asset.Room.Camera()
                {
                    Border = new Vector2(320, 240),
                    Rec = new Rectangle(Vector2.Zero, 640, 480),
                    Speed = Vector2.One * -1f
                }
            }],
            Instances = [],
            Layers = []
        };
        var handle = Room.Register(0, "RoomTest", room);
        
        GM.Room.Goto(handle);
    }
}
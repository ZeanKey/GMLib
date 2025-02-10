namespace GMLib;

public partial class GML
{
    internal static List<Room> RoomReserved = [];
    
    public static RenderTexture2D ApplicationSurface { get; internal set; }

    public static bool ApplicationSurfaceEnabled { get; set; } = true;
    
    public static int CurrentView { get; set; }
    
    public static Room CurrentRoom = GMLib.Room.Root;
    public static int RoomWidth => (int)CurrentRoom.Size.X;
    public static int RoomHeight => (int)CurrentRoom.Size.Y;
}

public class Room : ObjectManager
{
    public static readonly Room Root = new Room()
    {
        Size = Vector2.Zero
    };
    
    public Handle<Asset.Room>? Source { get; private set; }
    
    public Vector2 Size { get; set; }
    public bool Persistent { get; private set; }
    
    public bool EnableViews { get; set; }
    public bool ClearDisplayBuffer { get; set; }
    public bool ClearViewportBackground { get; set; }
    public Color Color { get; set; }

    public List<View> Views { get; } = [];
    public List<ILayer> Layers { get; } = [];
    
    public readonly List<Room> Children = [];
    
    private Room()
    {
    }
    
    public Room(Handle<Asset.Room> handle)
    {
        if (handle.Target is not { } asset) return;
        Source = handle;
        
        Size = asset.Size;
        Persistent = asset.Persistent;
        EnableViews = asset.EnableViews;
        ClearDisplayBuffer = asset.ClearDisplayBuffer;
        ClearViewportBackground = asset.ClearViewportBackground;
        Color = asset.Color;
        
        foreach (var view in asset.Views)
        {
            Views.Add(new View()
            {
                Camera = new Camera()
                {
                    Border = view.Camera.Border,
                    Speed = view.Camera.Speed,
                    View = view.Camera.Rec
                    // TODO Target
                },
                Viewport = new Viewport()
                {
                    Visible = view.Visible,
                    Rec = view.Port
                }
            });
        }
        
        foreach (var layer in asset.Layers)
        {
            // TODO
        }

        foreach (var inst in asset.Instances)
        {
            // TODO
        }
    }

    public void AddLayer(ILayer layer)
    {
        SubManagers.Add(layer);
        Layers.Add(layer);
        layer.Init();
    }

    public void RemoveLayer(ILayer layer)
    {
        SubManagers.Remove(layer);
        Layers.Remove(layer);
        layer.Uninit();
    }
    
    public void AddRoom(Room room)
    {
        SubManagers.Add(room);
        Children.Add(room);
    }

    public void RemoveRoom(Room room)
    {
        SubManagers.Remove(room);
        Children.Remove(room);
    }
    
    internal void Init()
    {
        foreach (var layer in Layers)
        {
            layer.Init();
            SubManagers.Add(layer);
        }
    }

    internal void Uninit()
    {
        foreach (var layer in Layers)
        {
            layer.Uninit();
            SubManagers.Remove(layer);
        }
    }
    
    public void Draw() 
    {
        EventPerform(ObjectEventType.PreDraw);
        DrawViews();
        foreach (var subRoom in Children)
        {
            subRoom.DrawViews();
        }
        EventPerform(ObjectEventType.PostDraw);

        RaylibExtensions.DrawTextureInversed(GML.ApplicationSurface);
        // DrawGUI(r);
        // DrawCursor(r);
    }
    
    public void DrawViews() 
    {
        if (ClearDisplayBuffer)
            Raylib.ClearBackground(Color);
        
        foreach (var view in Views)
        {
            view.Camera.Update();
        }
        
	    for (var i = 0; i < Views.Count; i++)
	    {
            var view = Views[i];
            if (!view.Enabled) continue;

            var viewSurfaceEnabled = view.Surface is not null;
            var viewSurface = view.Surface ?? (GML.ApplicationSurfaceEnabled ? GML.ApplicationSurface : null);
            
            if (viewSurface is not null)
            {
                Raylib.BeginTextureMode((RenderTexture2D)viewSurface);
            }
            if (!viewSurfaceEnabled)
            {
                Rlgl.Viewport(
                    (int)view.Viewport.Rec.X,
                    (int)view.Viewport.Rec.Y,
                    (int)view.Viewport.Rec.Width,
                    (int)view.Viewport.Rec.Height);
                Rlgl.EnableScissorTest();
                Rlgl.Scissor(
                    (int)view.Viewport.Rec.X,
                    (int)view.Viewport.Rec.Y,
                    (int)view.Viewport.Rec.Width,
                    (int)view.Viewport.Rec.Height);
            }

            if (ClearViewportBackground)
            {
                Raylib.ClearBackground(Color);
            }

            var target = view.Camera.View.Position + view.Camera.View.Size / 2f;
            var offset = view.Camera.View.Size / 2f;
            
            Raylib.BeginMode2D(new Camera2D() {
                Target = target,
                Offset = offset,
                Zoom = 1,
                Rotation = view.Camera.Angle 
            });
            view.Camera.Begin();
            {
                GML.CurrentView = i;
                EventPerform(ObjectEventType.Draw);
            }
            Raylib.EndMode2D();
            view.Camera.End(); 

            if (viewSurface is not null)
            {
                Raylib.EndTextureMode();
            }
            if (!viewSurfaceEnabled)
            {
                Rlgl.DisableScissorTest();
            }

            GML.CurrentView++;
	    }
	}
}
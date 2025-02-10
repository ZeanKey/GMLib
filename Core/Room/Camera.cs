namespace GMLib;

public class Camera : ICamera
{
    public float Angle { get; set; }
    
    public Vector2 Speed { get; set; }
    
    public Vector2 Border { get; set; }
    
    public Rectangle View { get; set; }
    
    public Matrix4x4 ViewMatrix { get; set; }
    
    public Matrix4x4 ProjectionMatrix { get; set; }

    public Handle<IObject>? Target { get; set; }

    public Matrix4x4 BuildMatrix()
    {
        throw new NotImplementedException();
    }
    
    public void UpdateMatrix()
    {
        // TODO
    }
    
    public void Update()
    {
        if (Target is not { } reference) return;
        if (reference.Target is not IObject2D inst) return;
        
        var halfwidth = View.Width * 0.5f;
        var halfheight = View.Height * 0.5f;
        
        var viewX = View.X;
        var viewY = View.Y;

        if (2 * Border.X >= View.Width)
            viewX = inst.Position.X - halfwidth;
        else if (inst.Position.X - Border.X < View.X)
            viewX = inst.Position.X - Border.X;
        else if (inst.Position.X + Border.X > View.X + View.Width)
            viewX = inst.Position.X + Border.X - View.Width;

        if (2 * Border.Y >= View.Height)
            viewY = inst.Position.Y - halfheight;
        else if (inst.Position.Y - Border.Y < View.Y)
            viewY = inst.Position.Y - Border.Y;
        else if (inst.Position.Y + Border.Y > View.Y + View.Height)
            viewY = inst.Position.Y + Border.Y - View.Height;


        // Make sure it does not extend beyond the room
        var roomSize = GML.CurrentRoom.Size;
        
        if (viewX < 0) viewX = 0;
        if (viewX + View.Width > roomSize.X) viewX = roomSize.X - View.Width;
        if (viewY < 0) viewY = 0;
        if (viewY + View.Height > roomSize.Y) viewY = roomSize.Y - View.Height;

        // Restrict motion speed
        if (Speed.X >= 0) 
        {
            if (viewX < View.X && View.X - viewX > Speed.X) viewX = View.X - Speed.X;
            if (viewX > View.X && viewX - View.X > Speed.X) viewX = View.X + Speed.X;
        }
        if (Speed.Y >= 0) 
        {
            if (viewY < View.Y && View.Y - viewY > Speed.Y) viewY = View.Y - Speed.Y;
            if (viewY > View.Y && viewY - View.Y > Speed.Y) viewY = View.Y + Speed.Y;
        }

        View = View with { X = viewX, Y = viewY };
        
        UpdateMatrix();
    }

    public void Begin()
    {
        
    }

    public void End()
    {
        
    }
}
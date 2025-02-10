namespace GMLib;

public struct RoomInfo
{
    public int Width;
    public int Height;
    public Asset.Script.Function CreationCode;
    public bool PhysicsWorld;
    public float PhysicsGravityX;
    public float PhysicsGravityY;
    public float PhysicsPixToMeters;
    public bool Persistent;
    public bool EnableViews;
    public bool ClearDisplayBuffer;
    public bool ClearViewportBackground;
    public Color Colour;
    public InstanceInfo[] Instances;
    public LayerInfo[] Layers;
    public ViewInfo[] Views;
}

public struct InstanceInfo
{
    public Asset.Object Id;
    public string ObjectIndex;
    
}

public struct LayerInfo
{
    
}

public struct ViewInfo
{

}
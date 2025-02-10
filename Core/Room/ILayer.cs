namespace GMLib;

public interface ILayer : IObjectManager
{
    public Action? BeginScript { get; set; }
    public Action? EndScript { get; set; }
    
    public int Depth { get; set; }
    public bool Visible { get; set; }

    public void Destroy();

    public void Begin() 
        => BeginScript?.Invoke();

    public void End() 
        => EndScript?.Invoke();

    public void Init();

    public void Uninit();
}

public interface ILayer<T> : ILayer where T : Asset.Room.Element
{
    
}
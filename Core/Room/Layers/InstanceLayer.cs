namespace GMLib;

public class InstanceLayer : ObjectManager, ILayer<Asset.Room.Element.Instance>
{
    public int Depth { get; set; }
    public bool Visible { get; set; }
    
    public Action? BeginScript { get; set; }
    public Action? EndScript { get; set; }
    
    public void Destroy()
    {
        Uninit();
    }

    public void Init()
    {
        
    }

    public void Uninit()
    {
        
    }
    
    public override void EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        if (type == ObjectEventType.Draw)
        {
            if (!Visible) return;
            (this as ILayer).Begin();
        }

        foreach (var inst in Contents.Values)
        {
            inst.EventPerform(type, eventArgs);
        }
        foreach (var submanager in SubManagers)
            submanager.EventPerform(type, eventArgs);
        
        if (type == ObjectEventType.Draw)
            (this as ILayer).End();
    }
}
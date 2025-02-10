namespace GMLib;

/// <summary>
/// The base object of the template when construct user's own game object class.<br/>
/// </summary>
public class GameObject: Object2D
{
    /// <summary>
    /// 
    /// </summary>
    public virtual void Create()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Step()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Draw()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void DrawGUI()
    {
        
    }
    
    public virtual void CleanUp()
    {
        
    }
    
    public virtual void Destroy()
    {

    }

    public override bool EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        switch (type)
        {
            case ObjectEventType.Create:
                Create();
                return true;
            case ObjectEventType.Step:
                Step();
                return true;
            case ObjectEventType.Draw:
                Draw();
                return true;
            case ObjectEventType.Destroy:
                Destroy();
                return true;
            case ObjectEventType.CleanUp:
                CleanUp();
                return true;
            default:
                return false;
        }
    }
    
    public static implicit operator Handle<IObject>?(GameObject gameObject) => gameObject.Id;
}
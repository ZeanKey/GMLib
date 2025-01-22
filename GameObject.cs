namespace GMLib;

/// <summary>
/// The base object of the template when construct user's own game object class.<br/>
/// </summary>
public class GameObject: Object
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
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void Destroy()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void CleanUp()
    {
        
    }
    
    public override bool InstanceExists()
    {
        return GML.Object.InstanceExists(Id);
    }
    
    public override bool InstanceDestroy()
    {
        return GML.Object.InstanceDestroy(Id);
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
namespace GMLib;

/// <summary>
/// The base object of the template when construct user's own game object class.<br/>
/// </summary>
public class GameObject: Object
{
    /// <summary>
    /// 
    /// </summary>
    public virtual void EventCreate()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void EventStep()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void EventDraw()
    {
        
    }
    
    /// <summary>
    /// 
    /// </summary>
    public virtual void EventDrawGUI()
    {
        
    }
    
    public virtual void EventDestroy()
    {
        
    }
    
    public virtual void EventCleanUp()
    {
        
    }
    
    public override bool Exists()
    {
        return GML.Object.Contains(Id);
    }
    
    public override bool Destroy()
    {
        return GML.Object.Remove(Id);
    }

    public override bool EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        switch (type)
        {
            case ObjectEventType.Create:
                EventCreate();
                return true;
            case ObjectEventType.Step:
                EventStep();
                return true;
            case ObjectEventType.Draw:
                EventDraw();
                return true;
            case ObjectEventType.Destroy:
                EventDestroy();
                return true;
            case ObjectEventType.CleanUp:
                EventCleanUp();
                return true;
            default:
                return false;
        }
    }
    
    public static implicit operator Handle<IObject>?(GameObject gameObject) => gameObject.Id;
}
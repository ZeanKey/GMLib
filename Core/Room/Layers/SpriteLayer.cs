namespace GMLib;

public class SpriteLayer : ILayer<Asset.Room.Element.Sprite>
{
    public readonly List<LayerElement.Sprite> Elements = [];
    
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
    
    public void EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        if (type == ObjectEventType.Draw)
        {
            if (!Visible) return;
            (this as ILayer).Begin();
            foreach (var spr in Elements)
            {
                spr.Draw();
            }
            (this as ILayer).End();
        }
    }

    public LayerElement.Sprite Add(Vector2 position, Handle<Asset.Sprite> sprite)
    {
        var ret = new LayerElement.Sprite()
        {
            Position = position,
            Source = sprite
        };
        Elements.Add(ret);
        return ret;
    }
    
    #region IObjectManager
    public Dictionary<Handle<IObject>, IObject> MergedCollection => [];
    public List<IObjectManager> SubManagers => [];

    public bool TryFindObject(Handle<IObject>? id, out WeakReference? inst)
    {
        inst = null;
        return false;
    }

    public Handle<IObject>? Add(IObject gMObject)
        => null;

    public bool Remove(Handle<IObject>? id)
        => false;

    public bool Contains(Handle<IObject>? id)
        => false;

    #endregion
}
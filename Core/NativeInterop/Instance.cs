using System.Dynamic;
using System.Reflection;
using GMLib.GM.Extensions;

namespace GMLib;

partial class GML
{
    public static bool VariableInstanceExists(Handle<IObject> instanceId, string name) 
        => instanceId.VariableExists(name);

    public static string[] VariableInstanceGetNames(Handle<IObject> instanceId) 
        => instanceId.GetVariableNames();

    public static int VariableInstanceGetCount(Handle<IObject> instanceId) 
        => instanceId.GetVariableNamesCount();

    public static object? VariableInstanceGet(Handle<IObject> instanceId, string name) 
        => instanceId.GetVariable(name);

    public static void VariableInstanceSet(Handle<IObject> instanceId, string name, object? val) 
        => instanceId.SetVariable(name, val);
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ReservedInstanceField : Attribute
{
    public string Name { get; init; }
    public ReservedInstanceField(string name)
    {
        Name = name;
    }
}

public class Instance : DynamicObject, IGameObject
{
    public static readonly Dictionary<string, MemberInfo> ReservedInstanceFields = [];
    
    static Instance()
    {
        var members = typeof(Instance).GetMembers()
            .Where(p => Attribute.IsDefined(p, typeof(ReservedInstanceField)));
       
        foreach (var member in members)
            ReservedInstanceFields.Add
            (
                (member.GetCustomAttribute(typeof(ReservedInstanceField))! as ReservedInstanceField)!.Name,
                member
            );
    }

    public Instance(Asset.Object objectAsset)
    {
        Self = new Dictionary<string, object?>();
        ObjectIndex = objectAsset;
    }
    
    #region Dynamic Object
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        return Self?.TryGetValue(binder.Name, out result) ?? throw new Exception();
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        if (Self is { } resource)
        {
            resource[binder.Name] = value;
            return true;
        }
        return false;
    }

    #endregion

    #region Reserved Fields
    [ReservedInstanceField("id")]
    public Handle<IObject>? Id { get; internal set; }
    
    [ReservedInstanceField("self")]
    public Struct? Self { get; internal set; }
    
    [ReservedInstanceField("object_index")]
    public Asset.Object ObjectIndex { get; private set; }
    
    [ReservedInstanceField("visible")]
    public bool Visible { get; set; }
    
    [ReservedInstanceField("solid")]
    public bool Solid { get; set; }
    
    [ReservedInstanceField("persistent")]
    public bool Persistent { get; set; }
    
    [ReservedInstanceField("depth")]
    public int Depth { get; set; }
    
    [ReservedInstanceField("layer")]
    public Asset.Room.Layer<Asset.Room.Element.Instance> Layer { get; set; }

    [ReservedInstanceField("alarm")] 
    public IGameObject.IndexedTimerAlarmManager Alarm { get; } = new();

    [ReservedInstanceField("direction")]
    public float Direction
    {
        get => -new Vector2(HSpeed, VSpeed).Angle();
        set
        {
            var velo = Vector2Extensions.FromAngle(-value) * MathF.Sqrt(MathF.Pow(HSpeed, 2) + MathF.Pow(VSpeed, 2));
            HSpeed = velo.X;
            VSpeed = velo.Y;
        }
    }
    
    [ReservedInstanceField("friction")]
    public float Friction { get; set; }
    
    [ReservedInstanceField("gravity")]
    public float Gravity { get; set; }
    
    [ReservedInstanceField("gravity_direction")]
    public float GravityDirection{ get; set; }

    [ReservedInstanceField("hspeed")]
    public float HSpeed { get; set; }
    
    [ReservedInstanceField("vspeed")]
    public float VSpeed { get; set; }

    [ReservedInstanceField("speed")]
    public float Speed
    {
        get => MathF.Sqrt(MathF.Pow(HSpeed, 2) + MathF.Pow(VSpeed, 2));
        set
        {
            var norm = new Vector2(HSpeed, VSpeed).Normalized();
            HSpeed = norm.X * value;
            VSpeed = norm.Y * value;
        }
    }
    
    [ReservedInstanceField("xstart")]
    public float XStart { get; init; }
    
    [ReservedInstanceField("ystart")]
    public float YStart { get; init; }
    
    [ReservedInstanceField("x")]
    public float X { get; set; }
    
    [ReservedInstanceField("y")]
    public float Y { get; set; }
    
    [ReservedInstanceField("xprevious")]
    public float XPrevious { get; set; }
    
    [ReservedInstanceField("yprevious")]
    public float YPrevious { get; set; }
    
    [ReservedInstanceField("sprite_index")]
    public Handle<Asset.Sprite> SpriteIndex { get; set; }

    [ReservedInstanceField("sprite_width")]
    public float SpriteWidth => ((IGameObject)this).SpriteSize.X;
    
    [ReservedInstanceField("sprite_height")]
    public float SpriteHeight => ((IGameObject)this).SpriteSize.Y;
    
    [ReservedInstanceField("sprite_xoffset")]
    public float SpriteXOffset => ((IGameObject)this).SpriteOffset.X;

    [ReservedInstanceField("sprite_yoffset")]
    public float SpriteYOffset => ((IGameObject)this).SpriteOffset.Y;
    
    [ReservedInstanceField("image_alpha")]
    public float ImageAlpha { get; set; }
    
    [ReservedInstanceField("image_angle")]
    public float ImageAngle { get; set; }
    
    [ReservedInstanceField("image_blend")]
    public Color3B ImageBlend { get; set; }
    
    [ReservedInstanceField("image_index")]
    public int ImageIndex { get; set; }

    [ReservedInstanceField("image_number")]
    public int ImageNumber => ((IGameObject)this).ImageNumber;
    
    [ReservedInstanceField("image_speed")]
    public float ImageSpeed { get; set; }
    
    [ReservedInstanceField("image_xscale")]
    public float ImageXScale { get; set; }
    
    [ReservedInstanceField("image_yscale")]
    public float ImageYScale { get; set; }
    
    [ReservedInstanceField("mask_index")]
    public Handle<Asset.Sprite> MaskIndex { get; set; }
    
    [ReservedInstanceField("bbox_bottom")]
    public float BBoxBottom { get; set; }
    
    [ReservedInstanceField("bbox_left")]
    public float BBoxLeft { get; set; }
    
    [ReservedInstanceField("bbox_right")]
    public float BBoxRight { get; set; }
    
    [ReservedInstanceField("bbox_top")]
    public float BBoxTop { get; set; }
    #endregion

    #region IGameObject
    public Vector2 Position
    {
        get => new(X, Y);
        set { X = value.X; Y = value.Y; }
    }

    public Vector2 Velocity
    {
        get => new(HSpeed, VSpeed);
        set { HSpeed = value.X; VSpeed = value.Y; }
    }
    
    public Vector2 PositionStart => new Vector2(XStart, YStart);

    public Vector2 PositionPrevious
    {
        get => new(XPrevious, YPrevious);
        set { XPrevious = value.X; YPrevious = value.Y; }
    }

    public Vector2 ImageScale
    {
        get => new(ImageXScale, ImageYScale);
        set { ImageXScale = value.X; ImageYScale = value.Y; }
    }

    public Rectangle BBox
    {
        get => new(BBoxLeft, BBoxTop, BBoxRight - BBoxLeft, BBoxBottom - BBoxTop);
        set
        {
            BBoxLeft = value.X;
            BBoxTop = value.Y;
            BBoxRight = value.X + value.Width;
            BBoxBottom = value.Y + value.Height;
        }
    }
    
    Vector2 IGameObject.Gravity
    {
        get => Vector2Extensions.FromAngle(-GravityDirection / 180f * MathF.PI) * Gravity;
        set
        {
            Gravity = value.Length();
            GravityDirection = -value.Angle() / MathF.PI * 180f;
        }
    }

    Color IGameObject.ImageBlend
    {
        get => (Color)ImageBlend with { A = (byte)(ImageAlpha * 255) };
        set
        {
            ImageBlend = new Color3B(value);
            ImageAlpha = value.A / 255f;
        }
    }

    #endregion

    #region IObject
    Handle<IObject>? IObject.Id { get => Id; set => Id = value; }

    public bool EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        return ObjectIndex.EventPerform(this, this, type, eventArgs);
    }

    public bool Destroy()
    {
        Self = null;
        return GML.Instance.Remove(Id);
    }

    public bool Exists()
    {
        return GML.Instance.Contains(Id);
    }

    public static explicit operator Handle<IObject>?(Instance instance)
    {
        return instance.Id;
    }
    
    #endregion
}

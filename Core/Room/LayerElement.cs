using GMLib.GM;

namespace GMLib;

public class LayerElement
{
    public class Sprite
    {
        public Handle<Asset.Sprite> Source;

        public Vector2 Position;
        public Vector2 Scale;
        public int Index;
        public float Speed;
        public float Angle;
        public Color Blend;
        
        public void Draw()
        {
            Source.DrawExt(Index, Position, Scale, Angle, Blend);
        }
    }
}
using static GMLib.RaylibExtension;
using System.Numerics;

namespace GMLib;

/// <summary>
/// 
/// </summary>
/// <param name="Position"></param>
/// <param name="Size"></param>
/// <param name="Offset"></param>
/// <param name="NormalizationRatio"></param>
public record struct SpriteUVs
(
    Vector2 Position,
    Vector2 Size,
    Vector2 Offset,
    Vector2 NormalizationRatio
);

/// <summary>
/// 
/// </summary>
public enum SpriteSpeedType
{
    FramesPerSecond,
    FramesPerGameFrame
}

public partial class Asset
{

    [AssetClassOf(AssetType.Sprite)]
    public sealed class Sprite : Asset, IHandleable<Sprite>, IDisposable
    {   
        public class Frame(Texture2D texture, Vector2 position)
        {
            public Texture2D Texture = texture;
            public Vector2 Position = position;
        }
        
        private Frame[] Frames; // The sub-images in the sprite
        
        public int Number => Frames.Length;
        public float Speed { get; set; }
        public SpriteSpeedType SpeedType { get; set; }
        public Vector2 Size { get; private set; }
        public Vector2 Offset { get; set; }
        public Vector2 BBox => throw new NotImplementedException();
        public Texture2D Texture => throw new NotImplementedException();
        public SpriteUVs UVs => throw new NotImplementedException();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public Frame this[float index] => this[(int)index];
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public Frame this[int index] => Frames[index % Number];

        public Sprite(string name, Texture2D texture, int imgnum, Vector2 orig) : base(name)
        {
            Offset = orig;
            Frames = new Frame[imgnum];
            Size   = new Vector2(texture.Width / (float)imgnum, texture.Height);
            for (var i = 0; i < imgnum; i++)
                Frames[i] = new Frame(texture, new Vector2(Size.X * i, 0));
        }

        #region Drawing
        public void Draw(int subimg, Rectangle source, Rectangle dest, Vector2 orig, float rot, Color c1, Color c2, Color c3, Color c4)
        {
            DrawTexturePro(this[subimg].Texture, source, dest, orig, rot, c1, c2, c3, c4);
        }
        
        public void Draw(int subimg, Rectangle source, Rectangle dest, float rot, Color c1, Color c2, Color c3, Color c4)
        {
            DrawTexturePro(this[subimg].Texture, source, dest, Offset, rot, c1, c2, c3, c4);
        }
        
        public void Draw(int subimg, Rectangle dest, Color colour)
        {
            var frame = this[subimg];
            Draw(subimg, new(frame.Position, Size), dest, 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Rectangle dest)
        {
            var colour = GML.GraphicManager.Color;
            var frame = this[subimg];
            Draw(subimg, new(frame.Position, Size), dest, 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Rectangle source, Vector2 position, Vector2 scale, Color colour)
        {
            Draw(subimg, source, new(position, Size * scale), 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Rectangle source, Vector2 position)
        {
            var colour = GML.GraphicManager.Color;
            Draw(subimg, source, new(position, Size), 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Vector2 position, Vector2 scale, float rot, Color colour)
        {
            var frame = this[subimg];
            Draw(subimg, new Rectangle(frame.Position, Size), new Rectangle(position, Size * scale), rot, colour, colour, colour, colour);
        }

        public void Draw(int subimg, Vector2 position)
        {
            var colour = GML.GraphicManager.Color;
            Draw(subimg, position, new Vector2(1, 1), 0, colour);
        }
        #endregion
        
        public static Sprite? ResolveHandle(Handle<Sprite> handle)
        {
            GML.Sprite.TryFindAsset(handle, out var sprite);
            return sprite;
        }
        
        #region Disposable
        private bool _disposedValue;
        ~Sprite() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            // TODO
            _disposedValue = true;
        }
        #endregion
    }
}
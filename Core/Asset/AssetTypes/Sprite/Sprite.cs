using static GMLib.RaylibExtensions;

namespace GMLib;

partial class GML
{
    public static AssetManager<Asset.Sprite> Sprite => Asset.GetManager<Asset.Sprite>();

    internal static readonly SpriteTextureManager SpriteTexture = new();
}

internal class SpriteTextureManager
{
    internal readonly Dictionary<uint, int> RefCountedTextures = [];

    public void Register(Texture2D texture)
    {
        if (!RefCountedTextures.TryAdd(texture.Id, 1))
            RefCountedTextures[texture.Id]++;
    }
    
    public void Remove(Texture2D texture)
    {
        RefCountedTextures[texture.Id]--;
        
        if (RefCountedTextures[texture.Id] == 0)
            Raylib.UnloadTexture(texture);
    }
}

partial class Asset
{
    public sealed class Sprite : Asset, IHandleable<Sprite>, IDisposable
    {   
        public class Frame(Image image, Vector2 position) : IDisposable
        {
            public readonly Vector2 Position = position;
            public readonly Image Image = image;
            
            #region Disposable
            private bool DisposedValue;
            
            ~Frame() => Dispose(false);

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        
            private void Dispose(bool disposing)
            {
                if (DisposedValue) return;
                Raylib.UnloadImage(image);
                DisposedValue = true;
            }
            #endregion
        }
        
        internal readonly Frame[] Frames; // The sub-images in the sprite
        
        public int Number => Frames.Length;
        public float Speed { get; set; }
        public SpriteSpeedType SpeedType { get; set; }
        public Vector2 Size { get; private set; }
        public Vector2 Offset { get; set; }
        public Vector2 BBox => throw new NotImplementedException();
        public Texture2D Texture { get; }
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

        public Sprite(Texture2D texture, (Vector2, Image)[] images, Vector2 orig)
        {
           Offset = orig;
           Frames = new Frame[images.Length];
           Texture = texture;
           
           var sample = images.First().Item2;
           Size = new Vector2(sample.Width, sample.Height);
           
           for (var i = 0; i < images.Length; i++)
               Frames[i] = new Frame(images[i].Item2, images[i].Item1);
        }
        
        #region Drawing
        public void Draw(int subimg, Rectangle source, Rectangle dest, Vector2 orig, float rot, Color c1, Color c2, Color c3, Color c4)
        {
            DrawTexturePro(Texture, source, dest, orig, rot, c1, c2, c3, c4);
        }
        
        public void Draw(int subimg, Rectangle source, Rectangle dest, float rot, Color c1, Color c2, Color c3, Color c4)
        {
            DrawTexturePro(Texture, source, dest, Offset, rot, c1, c2, c3, c4);
        }
        
        public void Draw(int subimg, Rectangle dest, Color colour)
        {
            var frame = this[subimg];
            Draw(subimg, new(frame.Position, Size), dest, 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Rectangle dest)
        {
            var colour = GML.Graphics.Color;
            var frame = this[subimg];
            Draw(subimg, new(frame.Position, Size), dest, 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Rectangle source, Vector2 position, Vector2 scale, Color colour)
        {
            Draw(subimg, source, new(position, Size * scale), 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Rectangle source, Vector2 position)
        {
            var colour = GML.Graphics.Color;
            Draw(subimg, source, new(position, Size), 0, colour, colour, colour, colour);
        }
        
        public void Draw(int subimg, Vector2 position, Vector2 scale, float rot, Color colour)
        {
            var frame = this[subimg];
            Draw(subimg, new Rectangle(frame.Position, Size), new Rectangle(position, Size * scale), rot, colour, colour, colour, colour);
        }

        public void Draw(int subimg, Vector2 position)
        {
            var colour = GML.Graphics.Color;
            Draw(subimg, position, new Vector2(1, 1), 0, colour);
        }
        #endregion
        
        public static Sprite Create(string filename, int imageNum, Vector2 origin)
        {
            var texture = Raylib.LoadTexture(filename);
            GML.SpriteTexture.Register(texture);

            var image = Raylib.LoadImage(filename);
            var images = new (Vector2, Image)[imageNum];
            var size = new Vector2(texture.Width / (float)imageNum, texture.Height);
            for (var i = 0; i < imageNum; i++)
            {
                var position = new Vector2(size.X * i, 0);
                images[i] = (position, Raylib.ImageFromImage(image, new Rectangle(position, size)));
            }
            var sprite = new Sprite(texture, images, origin);
            Raylib.UnloadImage(image);
            return sprite;
        }
    
        public static async Task<Sprite> CreateAsync(string filename, int imageNum, Vector2 origin)
        {
            var texture = Raylib.LoadTexture(filename);
        
            var t = Task.Run(() => 
            {
                GML.SpriteTexture.Register(texture);

                var image = Raylib.LoadImage(filename);
                var images = new (Vector2, Image)[imageNum];
                var size = new Vector2(texture.Width / (float)imageNum, texture.Height);
                for (var i = 0; i < imageNum; i++)
                {
                    var position = new Vector2(size.X * i, 0);
                    images[i] = (position, Raylib.ImageFromImage(image, new Rectangle(position, size)));
                }
            
                var sprite = new Sprite(texture, images, origin);
                Raylib.UnloadImage(image);
                return sprite;
            });
        
            return await t;
        }
        
        public static WeakReference? ResolveHandle(Handle<Sprite> handle)
        {
            return GML.Sprite.TryFindAsset(handle, out var sprite) ? sprite : null;
        }
        
        #region Disposable
        private bool DisposedValue;
        ~Sprite() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (DisposedValue) return;
            foreach (var frame in Frames)
                frame.Dispose();
            GML.SpriteTexture.Remove(Texture);
            DisposedValue = true;
        }
        #endregion

        static Sprite()
        {
            AssetManager.Managers[typeof(Sprite)] = new AssetManager<Sprite>();
        }
    }
}
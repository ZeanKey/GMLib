using System.Numerics;

namespace GMLib;

partial class Asset
{
    public class Room : Asset, IHandleable<Room>
    {
        public static Room First => throw new NotImplementedException();
        
        public static Room Last => throw new NotImplementedException();
        
        public static Room Next => throw new NotImplementedException();
        
        public static Room Previous => throw new NotImplementedException();
        
        public Vector2 Size { get; set; }
        
        public bool Persistent { get; set; }
        
        public bool ViewEnabled { get; set; }
        
        public List<ViewportCameraPair> ViewportCameras = [];
        
        public List<Viewport> Viewports 
            => ViewportCameras.Select(camera => camera.Viewport).ToList();
        
        public List<ICamera> Cameras 
            => ViewportCameras.Select(camera => camera.Camera).ToList();
        
        public Room()
        {

        }

        public static WeakReference? ResolveHandle(Handle<Room> handle)
        {
            throw new NotImplementedException();
        }
        
        public class Layer
        {
        
        }
    }
}
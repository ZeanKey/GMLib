namespace GMLib.GM.Extensions;

public static class Room
{
	public static bool Exists(this Handle<GMLib.Asset.Room> index)
		=> GML.Room.Contains(index);

	public static RoomInfo GetInfo(this Handle<GMLib.Asset.Room> index)
	{
		// TODO
		throw new NotImplementedException();
	}
	
	public static Handle<GMLib.Asset.Room> Duplicate(this Handle<GMLib.Asset.Room> index)
	{
		// TODO
		throw new NotImplementedException();
	}

	public static void Assign(this Handle<GMLib.Asset.Room> index, Handle<GMLib.Asset.Room> source)
	{
		// TODO
	}

	public static void InstanceAdd(this Handle<GMLib.Asset.Room> index, Vector2 position, Handle<GMLib.Asset.Object> obj)
	{
		// TODO
	}

	public static void InstanceClear(this Handle<GMLib.Asset.Room> index)
	{
		// TODO
	}

	public static void SetHeight(this Handle<GMLib.Asset.Room> index, int h)
	{
		
	}
	
	public static void SetWidth(this Handle<GMLib.Asset.Room> index, int h)
	{
		
	}
	
	public static void SetPersistent(this Handle<GMLib.Asset.Room> index, bool val)
	{
		
	}
	
	public static void SetViewEnabled(this Handle<GMLib.Asset.Room> index, bool val)
	{
		
	}
	
	public static void SetViewport(this Handle<GMLib.Asset.Room> index, int viewIndex, Viewport port)
	{
		
	}
	
	public static Viewport GetViewport(this Handle<GMLib.Asset.Room> index, int viewIndex)
	{
		throw new NotImplementedException();
	}

	public static void SetCamera(this Handle<GMLib.Asset.Room> index, int viewIndex, GMLib.Asset.Room.Camera camera)
	{
		if (index.Target is not { } room) return;
		room.Views[viewIndex].Camera = camera;
	}

	public static GMLib.Asset.Room.Camera? GetCamera(this Handle<GMLib.Asset.Room> index, int viewIndex)
	{
		return index.Target is not { } room ? null : room.Views[viewIndex].Camera;
	}
}
using GMLib.GM.Extensions;

namespace GMLib.GM;

public class Room
{
	public static Handle<GMLib.Asset.Room> First 
		=> GML.Room.Order[0];
	
	public static Handle<GMLib.Asset.Room> Last 
		=> GML.Room.Order[^1];

	public static Handle<GMLib.Asset.Room>? Next(int numb)
	{
		if (GML.Room.Order.Count <= 1) return null;
		if (numb != -1 && numb + 1 < GML.Room.Order.Count)
			return GML.Room.Order[numb + 1];
		return null;
	}
	
	public static Handle<GMLib.Asset.Room>? Next(Handle<GMLib.Asset.Room> handle)
	{
		return Next(GML.Room.Order.IndexOf(handle));
	}
	
	public static Handle<GMLib.Asset.Room>? Previous(int numb)
	{
		if (GML.Room.Order.Count <= 1) return null;
		if (numb != -1 && numb > 0)
			return GML.Room.Order[numb - 1];
		return null;
	}
	
	public static Handle<GMLib.Asset.Room>? Previous(Handle<GMLib.Asset.Room> handle)
	{
		return Previous(GML.Room.Order.IndexOf(handle));
	}
	
	public static Vector2 Size
		=> GML.CurrentRoom.Size;

	public static int Height
		=> (int)Size.Y;

	public static int Width
		=> (int)Size.X;

	public static bool Persistent
		=> GML.CurrentRoom.Persistent;

	public static bool Exists(Handle<GMLib.Asset.Room> index)
		=> index.Exists();

	public static RoomInfo GetInfo(Handle<GMLib.Asset.Room> index)
		=> index.GetInfo();
	
    public static void Goto(Handle<GMLib.Asset.Room> index)
	{
		if (index.Target is not { } room) return;
		
		if (GML.CurrentRoom != GMLib.Room.Root)
		{
			GML.CurrentRoom.EventPerform(ObjectEventType.EndRoom, 0);

			if (!GML.CurrentRoom.Persistent)
			{
				GML.RoomReserved.Remove(GML.CurrentRoom);
				GML.CurrentRoom.Uninit();
			}

			GMLib.Room.Root.RemoveRoom(GML.CurrentRoom);
		}

	    GMLib.Room newRoom;
	    if (room.Persistent && GML.RoomReserved.FirstOrDefault(reserved => reserved.Source == index) is { } reservedRoom)
		    newRoom = reservedRoom;
	    else
	    {
		    newRoom = new GMLib.Room(index);
		    newRoom.Init();
			GML.RoomReserved.Add(newRoom);
	    }
	    
	    GMLib.Room.Root.AddRoom(newRoom);
	    GML.CurrentRoom = newRoom;
	    
	    GML.CurrentRoom.EventPerform(ObjectEventType.StartRoom,0);
	}

	public static void GotoNext()
	{
		if (GML.CurrentRoom.Source is { } resource && Next(resource) is { } room) 
			Goto(room);
	}

	public static void GotoPrevious()
	{
		if (GML.CurrentRoom.Source is { } resource && Previous(resource) is { } room) 
			Goto(room);
	}

	public static void Restart()
	{
		// TODO
	}
	
	public static Handle<GMLib.Asset.Room> Add()
	{
		// TODO
		throw new NotImplementedException();
	}
	
	public static Handle<GMLib.Asset.Room> Duplicate(Handle<GMLib.Asset.Room> index)
		=> index.Duplicate();

	public static void Assign(Handle<GMLib.Asset.Room> index, Handle<GMLib.Asset.Room> source)
		=> index.Assign(source);
	
	public static void InstanceAdd(Handle<GMLib.Asset.Room> index, Vector2 position, Handle<GMLib.Asset.Object> obj)
		=> index.InstanceAdd(position, obj);
	
	public static void InstanceClear(Handle<GMLib.Asset.Room> index)
		=> index.InstanceClear();	
	
	public static void SetHeight(Handle<GMLib.Asset.Room> index, int height)
		=> index.SetHeight(height);
	
	public static void SetWidth(Handle<GMLib.Asset.Room> index, int width)
		=> index.SetWidth(width);
	
	public static void SetPersistent(Handle<GMLib.Asset.Room> index, bool value)
		=> index.SetPersistent(value);
	
	public static void SetViewEnabled(Handle<GMLib.Asset.Room> index, bool value)
		=> index.SetViewEnabled(value);
	
	public static void SetViewport(Handle<GMLib.Asset.Room> index, int viewIndex, Viewport port)
		=> index.SetViewport(viewIndex, port);
	
	public static Viewport GetViewport(Handle<GMLib.Asset.Room> index, int viewIndex)
		=> index.GetViewport(viewIndex);

	public static void SetCamera(Handle<GMLib.Asset.Room> index, int viewIndex, GMLib.Asset.Room.Camera camera)
		=> index.SetCamera(viewIndex, camera);

	public static GMLib.Asset.Room.Camera? GetCamera(Handle<GMLib.Asset.Room> index, int viewIndex)
		=> index.GetCamera(viewIndex);

	public static implicit operator GMLib.Room(Room _)
		=> GML.CurrentRoom;
}
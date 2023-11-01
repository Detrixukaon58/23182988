using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public partial class RoomManager : Node3D
{

	List<(Guid, Node3D)> rooms;

	[Export] string currentLevel;

	private static RoomManager currrentInstance;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rooms = new List<(Guid, Node3D)>();
		#if DEBUG
			string scene_path = GetTree().CurrentScene.SceneFilePath;
			currentLevel = new FileInfo(scene_path).Directory.Name;
			rooms.Add((Guid.NewGuid(), (Node3D)GetTree().CurrentScene));
		#endif
		currrentInstance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public static RoomManager GetCurrentInstance(){
		return currrentInstance;
	}

	public void LoadLevel(string levelName) {
		if(ResourceLoader.Load("res://Scenes/" + levelName + "/init.tscn") is PackedScene scene){
			foreach((Guid guid, Node3D room) in rooms){
				room.QueueFree();
			}
			rooms.Clear();
			rooms.Add((Guid.NewGuid(), scene.Instantiate<Node3D>()));
			currentLevel = levelName;
		}
		else{
			GD.PrintErr("Couldn't load scene " + levelName + ". Scene Either does not exist or has no init.tscn");
		}
	}

	public Node3D FindRoom(string target){
		foreach((Guid guid, Node3D room) in rooms){
			if(guid.ToString().Equals(target)){
				return room;
			}
		}
		return null;
	}

	private Array<Node3D> GetChildrenInGroup(Node3D parent, string group){
		Array<Node3D> children = new Array<Node3D>();
		foreach(Node3D child in parent.GetChildren()){
			if(child.IsInGroup(group)){
				children.Add(child);
			}
		}
		return children;
	}

	/// <summary>
	/// Creates a room andd returns a door to that room
	/// THE ROOM MUST HAVE A DOOR OR NULL IS RETURNED!!!
	/// </summary>
	/// <param name="scene"></param>
	/// <returns></returns>
	public Node3D SpawnRoom(PackedScene scene){
		Node3D result = null; // THIS CANNOT BE NULL AFTER THIS FUNCTION IS RUN!!!
		RandomNumberGenerator rng = new RandomNumberGenerator();
		Node3D room = scene.Instantiate<Node3D>();
		GetTree().CurrentScene.AddChild(room);
		// Pick a Door - The room musst have a Door or this will crash!!!
		Array<Node3D> doors = GetChildrenInGroup(room, "Doors");
		if(doors.Count != 0){
			int door_index = rng.RandiRange(0, doors.Count - 1);

			result = doors[door_index];
			
		}
		room.SetProcess(true);
		room.SetPhysicsProcess(true);
		room.Position = new Vector3(rng.Randf() * 1000.0f, rng.Randf() * 1000.0f, rng.Randf() * 1000.0f);
		rooms.Add((Guid.NewGuid(), room));

		return result;

	}
	public Node3D GetDoor() {
		Node3D result = null; // THIS CANNOT BE NULL AFTER THIS FUNCTION IS RUN!!!
		RandomNumberGenerator rng = new RandomNumberGenerator();
		int choice = rng.RandiRange(0, 1);

		switch(choice){
			case 0: // Previous Room
			{
				if(rooms.Count != 0){
					int room_index = rng.RandiRange(0, rooms.Count - 1);
					Node3D room = rooms[room_index].Item2;
					Array<Node3D> doors = GetChildrenInGroup(room, "Doors");
					if(doors.Count != 0){
						int door_index = rng.RandiRange(0, doors.Count - 1);

						result = doors[door_index];
						break;
					}
				}
				goto default;
			}

			default: // New Room
			{
				// Load The new room
				Node3D room = (Node3D)GetRoom()["room"];
				GetTree().CurrentScene.AddChild(room);
				Array<Node3D> doors = GetChildrenInGroup(room, "Doors");
				if(doors.Count != 0){
					int door_index = rng.RandiRange(0, doors.Count - 1);
					result = doors[door_index];
				}
				break;
			}
		}
		while(result == null){
			int room_index = rng.RandiRange(0, rooms.Count - 1);
			Node3D room = rooms[room_index].Item2;
			Array<Node3D> doors = GetChildrenInGroup(room, "Doors");
			if(doors.Count != 0){
				int door_index = rng.RandiRange(0, doors.Count - 1);

				result = doors[door_index];
				break;
			}
			else{
				// Cannnot possibly find another door for us to go through!!
				break;
			}
		}
		return result;
	}

	// Loads a new room for us to enter!!
	public Godot.Collections.Dictionary GetRoom(){
		DirAccess dir = DirAccess.Open("res://Scenes/" + currentLevel);

		var error = dir.ListDirBegin();
		if(error.HasFlag(Error.Failed)){
			GD.PrintErr("Error Findding level " + currentLevel + ". Maybe the level doesn't exist?");
			return null;
		}
		List<string> possibleLevels = new List<string>();
		List<string> possibleGoals = new List<string>();
		while(true){
			var nextDir = dir.GetNext();
			if(nextDir == ""){
				break;
			}
			if(nextDir.Contains("init")){
				continue;
			}
			if(nextDir.Contains("goal") || nextDir.Contains("Goal")){
				possibleGoals.Add(nextDir);
				continue;
			}
			possibleLevels.Add(nextDir);
		}

		RandomNumberGenerator rng = new RandomNumberGenerator();
		int choice = rng.RandiRange(0, 1);
		Node3D newRoom = null;
		switch(choice){
			case 0:
			{
				if(possibleGoals.Count != 0){
					int index = rng.RandiRange(0, possibleGoals.Count - 1);
					newRoom = ResourceLoader.Load<PackedScene>("res://Scenes/" + currentLevel + "/" + possibleGoals[index]).Instantiate<Node3D>();
					foreach((Guid guid, Node3D room) in rooms){
						if(room.Name == newRoom.Name){
							newRoom.QueueFree();
							index = rng.RandiRange(0, possibleLevels.Count - 1);
							newRoom = ResourceLoader.Load<PackedScene>("res://Scenes/" + currentLevel + "/" + possibleLevels[index]).Instantiate<Node3D>();
							break;
						}
					}
					break;
				}
				goto default;
			}
			default:
			{
				int index = rng.RandiRange(0, possibleLevels.Count - 1);
				newRoom = ResourceLoader.Load<PackedScene>("res://Scenes/" + currentLevel + "/" + possibleLevels[index]).Instantiate<Node3D>();
				break;
			}
		}
		newRoom.Position = new Vector3(rng.Randf() * 1000.0f, rng.Randf() * 1000.0f, rng.Randf() * 1000.0f);
		Guid newGuid = Guid.NewGuid();
		rooms.Add((newGuid, newRoom));
		Godot.Collections.Dictionary dict = new Godot.Collections.Dictionary
		{
			{ "guid", newGuid.ToString() },
			{ "room", newRoom },
		};
			
		
		return dict;

	}
}

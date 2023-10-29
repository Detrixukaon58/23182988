using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class RoomManager : Node
{

	List<(Guid, Node3D)> rooms;

	[Export] string currentLevel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
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
				int index = rng.RandiRange(0, possibleLevels.Count - 1);
				newRoom = ResourceLoader.Load<PackedScene>("res://Scenes/" + currentLevel + "/" + possibleLevels[index]).Instantiate<Node3D>();
				break;
			}
			default:
			{
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
		}
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

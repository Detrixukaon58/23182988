using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class Door : Node3D
{
	[Export] Door OtherDoor;
	[Export] bool isOpen;

	SubViewport viewport;
	MeshInstance3D tex;

	Camera3D cam;

	Area3D coollBox;

	Array<Node3D> enteredBodies = new Array<Node3D>();

	[Export] bool hasTellop;
	[Export] bool fetchesRoom = true;
	[Export] bool canSelfReflect;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		viewport = GetNode<SubViewport>("SubViewport");
		tex = GetNode<MeshInstance3D>("Sprite3D");
		cam = viewport.GetNode<Camera3D>("InteriorCamera");
		coollBox = GetNode<Area3D>("Area3D");
		if( OtherDoor != null) {
			// tex.Texture = OtherDoor.viewport.GetTexture();
			if(OtherDoor.OtherDoor == null){
				OtherDoor.OtherDoor = this;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		DebugDraw3D.DrawArrowLine(GlobalPosition + Vector3.Up, GlobalPosition + Transform.Basis.Z + Vector3.Up, Colors.Red, 0.2f);
		if(OtherDoor != null){
			bool isLooking = false;
			if(GetTree().Root.FindChild("Player", true, false) is CharacterBody3D Player){
				Vector3 direction = Player.Position - Position;
				Vector3 look = GetViewport().GetCamera3D().Transform.Basis.Z;
				if(direction.AngleTo(look) <= GetViewport().GetCamera3D().Fov){
					isLooking = true;
				}
			}
			if(cam == null){
				cam = viewport.GetNode<Camera3D>("InteriorCamera");
			}
			if(isLooking){
				if(!cam.Current){
					cam.MakeCurrent();
				}
				if(isOpen){
					SortCamera();
					GetNode<Node3D>("Door").Rotation = new Vector3(0.0f, 90.0f, 0.0f);
				}
				else if (OtherDoor.isOpen){
					SortCamera();
					GetNode<Node3D>("Door/DoorBone").Rotation = new Vector3(0.0f, 90.0f, 0.0f);
				}
				else{
					GetNode<Node3D>("Door/DoorBone").Rotation = new Vector3(0.0f, 0.0f, 0.0f);
					GetNode<Node3D>("Door").Rotation = new Vector3(0.0f, 0.0f, 0.0f);
				}
				#if TOOLS
				SortCamera();
				#endif
			}
			else{
				cam.Current = false;
			}
		}
	}

	public void SortCamera() {
		if(GetViewport().GetCamera3D() is Camera3D main && main != null){
			// GD.Print(main.Name);
			Vector3 xyPosition = new Vector3(main.GlobalPosition.X, 0.0f, main.GlobalPosition.Z);
			Vector3 forward = Quaternion.FromEuler(OtherDoor.Rotation) 
				* Quaternion.FromEuler(-Rotation) 
				* Quaternion.FromEuler(Vector3.Up * Vector3.Forward.SignedAngleTo(xyPosition - new Vector3(GlobalPosition.X, 0.0f, GlobalPosition.Z), Vector3.Up))
				* Vector3.Forward;
			cam.Position = OtherDoor.GlobalPosition - forward * (xyPosition - new Vector3(GlobalPosition.X, 0.0f, GlobalPosition.Z)).Length() + Vector3.Up 
				* (main.GlobalPosition.Y - GlobalPosition.Y);
			cam.Rotation = (Quaternion.FromEuler(OtherDoor.Rotation) 
				* Quaternion.FromEuler(-Rotation) 
				* Quaternion.FromEuler(new Vector3(0.0f, main.GlobalRotation.Y, 0.0f)) 
				* Quaternion.FromEuler(new Vector3(-main.GlobalRotation.X, 0.0f, -main.GlobalRotation.Z))
				* new Quaternion(Vector3.Up, Mathf.Pi)).GetEuler();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(enteredBodies.Count != 0){
			foreach(Node3D body in enteredBodies){
				if(body.Name == "Player" || body.IsInGroup("Player")){
					// GD.Print(Input.IsActionJustPressed("select"));
					if(Input.IsActionJustPressed("select")){
						if(OtherDoor == null){
							// need to create a new rom or select a room thats been used before
							// Use GetDooor from RoomManager
							OtherDoor = (Door)RoomManager.GetCurrentInstance().GetDoor();
							while((OtherDoor == this || OtherDoor == null) && !canSelfReflect){
								OtherDoor = (Door)RoomManager.GetCurrentInstance().GetDoor();
							}
							if(OtherDoor.OtherDoor == null){
								OtherDoor.OtherDoor = this;
							}
						}
						if(OtherDoor.isOpen){
							OtherDoor.isOpen = false;
							isOpen = false;
						}
						else{
							isOpen = !isOpen;
						}
					}
				}
			}
		}
	}

	public void BodyEnter(Node3D body){
		enteredBodies.Add(body);
	}

	public void BodyExit(Node3D body){
		enteredBodies.Remove(body);
	}

	public void Teleport(Node3D body){
		if(body.Name == "Player" || body.IsInGroup("Player")){
			if((isOpen || OtherDoor.isOpen) && !OtherDoor.hasTellop){
				body.GlobalPosition = OtherDoor.GlobalPosition + Vector3.Up;
				body.GlobalRotation = OtherDoor.GlobalRotation + (body.GlobalRotation - Rotation + Vector3.Up * Mathf.Pi);
				GD.Print(body.Rotation.Y - Rotation.Y + Mathf.Pi);
				GD.Print(body.Rotation);
				hasTellop = true;
			}
		}
	}

	public void TeleportExit(Node3D body) {
		if(OtherDoor.hasTellop && (body.Name == "Player" || body.IsInGroup("Player"))){
			OtherDoor.hasTellop = false;
		}
	}

	public void Visible(){
		cam.Current = false;
		cam.Hide();
	}

 /// <summary>
 /// THIS DOES NOT TURN THE DOOR INVISIBLE!!!!
 /// </summary>
	public void Invisible(){
		cam.Current = true;
		cam.Show();
	}
}

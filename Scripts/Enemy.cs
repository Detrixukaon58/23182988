using Godot;
using Godot.Collections;
using System;

[Tool]
public partial class Enemy : CharacterBody3D
{
	//NavigationAgent2D navigation;

	// Keep Enemy movement simple for now. It just sees the player then runs toward it!


	#region Stats 
	//Key stats for an enemy type (can be defined in seperate prefabs and loaded in quickly)
	float attackSpd;
	float attackPwr = 10.0f;
	
	float movementSpd;
	
	int maxHealth = 100;
	int health = 100;
	
	bool isLarge;

	
	float largeAttkPwr;
	float largeAttkSpd;

	#endregion

	#region MeshHandling
	AnimationTree anim;
	#endregion

	#region Ai
	bool isMoving;
	Node3D target = null;
	float targetLossTimer = 0.0f;
	
	float attackTmr;

	float viewAngle;
	float viewDepth;

	RayCast3D ForwardCheck;
	#endregion

	public override Array<Dictionary> _GetPropertyList()
	{
		Array<Dictionary> list = new Array<Dictionary>
		{
			PropEntry("attackSpd", Variant.Type.Float, PropertyHint.None),
			PropEntry("attackPwr", Variant.Type.Float, PropertyHint.None),
			PropEntry("movementSpd", Variant.Type.Float, PropertyHint.None),
			PropEntry("isLarge", Variant.Type.Bool, PropertyHint.None),
			PropEntry("viewAngle", Variant.Type.Float, PropertyHint.None),
			PropEntry("viewDepth", Variant.Type.Float, PropertyHint.None),
		};

		#if TOOLS
		if(isLarge == true){
			list.Add(PropEntry("largeAttkPwr", Variant.Type.Float, PropertyHint.None));
			list.Add(PropEntry("largeAttkSpd", Variant.Type.Float, PropertyHint.None));
		}
		#else
		// Want to add them anyway cause this is to make the edditor less cluttered for us!!
		list.Add(PropEntry("largeAttkPwr", Variant.Type.Float, PropertyHint.None));
		list.Add(PropEntry("largeAttkSpd", Variant.Type.Float, PropertyHint.None));
		#endif
		return list;
	}

		public static Dictionary PropEntry(string name, Variant.Type type, PropertyHint hint, string hintStr = "")
	{
		var d = new Dictionary();
		d.Add("name", name);
		d.Add("type", (int)type);
		d.Add("hint", (int)hint);
		d.Add("hint_string", hintStr);
		return d;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim = GetNode<AnimationTree>("AnimationTree");
		isMoving = false;
		ForwardCheck = GetNode<RayCast3D>("ForwardCheck");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		PhysicsShapeQueryParameters3D qParams = new PhysicsShapeQueryParameters3D
		{
			CollideWithAreas = true,
			CollideWithBodies = true,
			CollisionMask = CollisionMask,
			Exclude = new Array<Rid> { GetRid(), }
		};
		SphereShape3D shape = new SphereShape3D();
		shape.Radius = viewDepth;
		qParams.ShapeRid = shape.GetRid();
		qParams.Transform = GlobalTransform;

		var results = GetWorld3D().DirectSpaceState.IntersectShape(qParams);
		
		if(results != null && results.Count != 0){
			// We have found something on thisa collision plane!!
			// Now we test to see if they are the player!!
			foreach(var result in results){
				// GD.Print(result);
				if((Node)result["collider"] is CharacterBody3D rb && rb.Name == "Player"){
					
					Vector3 direction = (rb.GlobalPosition - GlobalPosition - Vector3.Up).Normalized();
					// GD.Print(Mathf.Abs((Transform.Basis.Z).AngleTo(direction)));
					if(Mathf.Abs((-Transform.Basis.Z).AngleTo(direction)) <= Mathf.DegToRad(viewAngle)){
						ForwardCheck.Position = Vector3.Up;
						ForwardCheck.Rotation = -Rotation;
						ForwardCheck.TargetPosition = direction * viewDepth;
						if(ForwardCheck.IsColliding() && ForwardCheck.GetCollider() is CharacterBody3D rb2 && rb2.Name == "Player"){
							// We can see the player!!
							target = rb;
							targetLossTimer = 10.0f; // 10 seconds till ennemy loses target
							
						}
					}
				}
			}
		}
		
		if(targetLossTimer > 0.0f && target != null){
			targetLossTimer -= (float) delta;
			Vector3 direction = (target.GlobalPosition - GlobalPosition - Vector3.Up).Normalized();
			Velocity = direction * movementSpd;
			
			MoveAndSlide();
			if(Velocity.LengthSquared() >= 0.1f){
				isMoving = true;
			}
			else{
				isMoving = false;
			}
			Vector3 dd = new Vector3(direction.X, 0.0f, direction.Z);
			Rotation = new Vector3(0.0f, Vector3.Forward.SignedAngleTo(dd, Vector3.Up), 0.0f);
			if (attackTmr < 0.0f)
			{
				Attack();
				attackTmr = attackSpd;
			}
			attackTmr -= (float) delta;
		}
		else{
			Velocity = Vector3.Zero;
			isMoving = false;
			anim.Set("attacking", false);
		}
		DebugDraw3D.DrawSphere(GlobalPosition, viewDepth, Colors.Aqua);
		DebugDraw3D.DrawArrowLine(GlobalPosition + Vector3.Up, GlobalPosition + -Transform.Basis.Z + Vector3.Up, Colors.Red);
		anim.Set("moving", isMoving);
	}

	public void Attack()
	{

		if(ForwardCheck.IsColliding() && Position.DistanceTo(target.Position) <= 2.0f){
			// We begin our attack!!
			
			anim.Set("attacking", true);
			if (ForwardCheck.GetCollider().HasMethod("hurt"))
			{
				((Node3D)ForwardCheck.GetCollider()).Call("hurt", attackPwr);
				// i do nor know how to call the method in a way that doesnt make godot complain
			}
		}
		else{
			anim.Set("attacking", false);
		}
	}
	
	public void die()
	{
		QueueFree();
	}
	
	public void hurt(int amount)
	{
		health -= amount;
		if (health <= 0)
		{
			die();
		}
		if (health > maxHealth)
		{
			health = maxHealth;
		}
		GD.Print($"health remainin' {health}");
	}
}

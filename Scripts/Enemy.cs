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
	float attackPwr;
	float movementSpd;

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

	float viewAngle;
	float viewDepth;

	RayCast3D ForwardCheck;
	#endregion

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> list = new Array<Dictionary>
        {
            PropEntry("attackSpd", Variant.Type.Float, PropertyHint.None),
			PropEntry("atackPwr", Variant.Type.Float, PropertyHint.None),
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
		qParams.Transform = Transform;

		var results = GetWorld3D().DirectSpaceState.IntersectShape(qParams);
		
        if(results != null && results.Count != 0){
			// We have found something on thisa collision plane!!
			// Now we test to see if they are the player!!
			foreach(var result in results){
				// GD.Print(result);
				if((Node)result["collider"] is CharacterBody3D rb && rb.Name == "Player"){
					
					Vector3 direction = (rb.Position - Position - Vector3.Up).Normalized();
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
			Vector3 direction = (target.Position - Position - Vector3.Up).Normalized();
			Velocity = direction * movementSpd;
			
			MoveAndSlide();
			isMoving = true;
			Vector3 dd = new Vector3(direction.X, 0.0f, direction.Z);
			Rotation = new Vector3(0.0f, Vector3.Forward.SignedAngleTo(dd, Vector3.Up), 0.0f);
		}
		else{
			Velocity = Vector3.Zero;
			isMoving = false;
		}
		DebugDraw3D.DrawSphere(Position, viewDepth, Colors.Aqua);
		DebugDraw3D.DrawArrowLine(Position + Vector3.Up, Position + -Transform.Basis.Z + Vector3.Up, Colors.Red);
		anim.Set("moving", isMoving);
    }
}

extends CharacterBody3D

@export var SENSITIVITY: float;
const SPEED = 5.0
const JUMP_VELOCITY = 4.5
var cam: Camera3D
var hit_scan: RayCast3D

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")

func _ready():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	cam = $PlayerCam
	hit_scan = get_node("HitScan");

func _process(delta):
	#if Input.action_press("shoot") :
		pass
	



func _physics_process(delta):
	# Add the gravity.
	if not is_on_floor():
		velocity.y -= gravity * delta

	# Handle Jump.
	if Input.is_action_just_pressed("space") and is_on_floor():
		velocity.y = JUMP_VELOCITY
	# Replace this with just playing the CruS jump noise because funny.
	# "Press Space to grunt"

	# Get the input direction and handle the movement/deceleration.
	var input_dir = Input.get_vector("leftstrafe", "rightstrafe", "forwards", "backwards")
	var direction = (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)
	
	move_and_slide();

func _input(event):
	if (event is InputEventMouseMotion and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED):
		cam.rotate_x(deg_to_rad(event.relative.y * -SENSITIVITY))
		rotate_y(deg_to_rad(event.relative.x * -SENSITIVITY))
		
		
extends Camera3D

@export var SENSITIVITY: float;
var player: CharacterBody3D;

# Called when the node enters the scene tree for the first time.
func _ready():
	#Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	player = get_node("..")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func _input(event):
	if event is InputEventMouseMotion and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		rotate_x(deg_to_rad(event.relative.y * -SENSITIVITY))
		player.rotate_y(deg_to_rad(event.relative.x * -SENSITIVITY))

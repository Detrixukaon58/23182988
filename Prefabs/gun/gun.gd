extends StaticBody3D

@export var fireRate: int = 1;
@export var recoil: int = 1;

@export var bulletCapacity: int = 1;
@export var reloadSpeed: int = 1;

@export var bulletDamage: int = 1;
@export var bulletDistance: float = 100.0;

const IDLE_ANIM_NAME = "Pistol_idle"
const FIRE_ANIM_NAME = "Pistol_fire"
const RELOAD_ANIM_NAME = "Pistol_reload"

@onready var bullet: RayCast3D = $bullet
@export var bulletArray: Array = []


func fire():
	bullet.target_position = bulletDistance * global_transform.basis.z.normalized()
	if bullet.is_colliding():
		print("yippee")
	print(bullet.global_position)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

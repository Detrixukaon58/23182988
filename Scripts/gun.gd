extends Node3D

@export var fireRate: int = 1;
@export var recoil: int = 1;

@export var bulletCapacity: int = 1;
@export var reloadSpeed: int = 1;

@export var bulletDamage: int = 1;
@export var bulletDistance: float = 100.0;

const IDLE_ANIM_NAME = "Pistol_idle"
const FIRE_ANIM_NAME = "Pistol_fire"
const RELOAD_ANIM_NAME = "Pistol_reload"

@onready var bullet: ShapeCast3D = $bullet

@onready var cam: Camera3D = get_node("../../..");
@export var bulletArray: Array = []


func fire():
	var cam_forward = Vector3.FORWARD;
	var endpoint = cam.global_position + cam_forward * bulletDistance;
	bullet.global_position = global_position
	bullet.target_position = Vector3.FORWARD * bulletDistance;
	if bullet.is_colliding():
		print("yippee")
	print(bullet.target_position)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

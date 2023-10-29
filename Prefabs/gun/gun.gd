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

@onready var bulletPrefab = load("res://Prefabs/gun/bullet.tscn") # replace it with the location of the bullet scene idk
@export var bulletArray: Array = []


func fire():
	var newBullet = bulletPrefab.instantiate()
	add_child(newBullet)
	var hits = newBullet.init(bulletDamage, bulletDistance, position, Vector3.FORWARD)
	print(hits)
	bulletArray.append(newBullet)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

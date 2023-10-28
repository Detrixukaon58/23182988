extends StaticBody3D

@export var fireRate: int = 1;
@export var recoil: int = 1;

@export var bulletCapacity: int = 1;
@export var reloadSpeed: int = 1;

@export var bulletDamage: int = 1;
@export var bulletTime: int = 1;

const IDLE_ANIM_NAME = "Pistol_idle"
const FIRE_ANIM_NAME = "Pistol_fire"
const RELOAD_ANIM_NAME = "Pistol_reload"

@onready var bulletPrefab = $bullet # replace it with the location of the bullet scene idk
@export var bulletArray: Array = []


func fire():
	var newBullet = bulletPrefab.new()
	newBullet.init(bulletDamage, bulletTime, Vector3.ZERO, Vector3.FORWARD)
	bulletArray.append(newBullet)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

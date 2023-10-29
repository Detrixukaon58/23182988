extends RayCast3D

var damage: int = 1;
var distance: float = 100.0;


@onready var bulletPrefab = load("res://Prefabs/gun/bullet.tscn")


func init(damage: int = 1, distance: float = 100.0, startLocation: Vector3 = Vector3.ZERO, direction: Vector3 = Vector3.FORWARD):
	self.damage = damage
	self.distance = distance
	
	if distance < 0:
		return
	position = startLocation;
	set_target_position(startLocation + distance*direction)
	
	if is_colliding():
		var collision = get_collider()
		var collisions: Array = [collision]
		
		var collision_pos = get_collision_point()
		var new_distance = collision_pos.distance_to(startLocation)
		
		var newBullet = bulletPrefab.instantiate()
		add_child(newBullet)
		collisions.append(newBullet.init(damage, distance - new_distance, collision_pos, direction))
		
		return collisions
	else:
		return
	

extends RayCast3D

var damage: int = 1;
var range: int = 1;


@onready var bulletPrefab = $bullet


func init(damage: int = 1, range: int = 1, startLocation: Vector3 = Vector3.ZERO, direction: Vector3 = Vector3.FORWARD):
	self.damage = damage
	self.range = range
	
	if range < 0:
		return
	position = startLocation;
	set_target_position(startLocation + range*direction)
	
	if is_colliding():
		var collision = get_collider()
		var collisions: Array = [collision]
		
		var collision_pos = get_collision_point()
		var distance = collision_pos.distance_to(startLocation)
		
		var newBullet = bulletPrefab.new()
		collisions.append(newBullet.init(damage, range - distance, collision_pos, direction))
		
		return collisions
	else:
		return

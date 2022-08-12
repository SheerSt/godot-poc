extends KinematicBody2D



# Declare member variables here. Examples:
# var a = 2
# var b = "text"

onready var tilemap = get_node("../../TileMap")
onready var audio_player = get_node("../../AudioStreamPlayer2D")
var bump_sound = preload("res://bump2.ogg")

var flip = false;
var speed = 1;

# Called when the node enters the scene tree for the first time.
func _ready():
	bump_sound.set_loop(false)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	if Input.is_key_pressed(KEY_X): speed = 2
	else: speed = 1

	if Input.is_action_pressed("ui_right"):
		_walk(Vector2.RIGHT)
	elif Input.is_action_pressed("ui_left"):
		_walk(Vector2.LEFT)
	elif Input.is_action_pressed("ui_up"):
		_walk(Vector2.UP)
	elif Input.is_action_pressed("ui_down"):
		_walk(Vector2.DOWN)
	else:
		flip = false
		$Sprite.set_flip_h(flip)


func _walk(direction):

	set_process(false)
	
	var cell_start = tilemap.world_to_map(position)
	var cell_target = cell_start + direction
	var target_position = tilemap.map_to_world(cell_target)

	var name = ""
	if direction == Vector2.RIGHT:
		name = "walk-right"
		$Sprite.set_flip_h(false)
	elif direction == Vector2.LEFT:
		name = "walk-left"
		$Sprite.set_flip_h(false)
	elif direction == Vector2.UP:
		name = "walk-up"
		$Sprite.set_flip_h(flip)
	elif direction == Vector2.DOWN:
		name = "walk-down"
		$Sprite.set_flip_h(flip)
	
	$AnimationPlayer.play(name)
	
	var collision = move_and_collide(direction * speed, true, true, true)
	
	if collision:
		speed = 1
		audio_player.stream = bump_sound
		audio_player.play()
	
	# while !position.is_equal_approx(target_position):
	for i in range(16 / speed):

		if not collision:
			position = position.move_toward(target_position, speed)
			
		yield(get_tree(), "idle_frame")
		
		collision = move_and_collide(direction * speed, true, true, true)


	$AnimationPlayer.stop()
	flip = !flip
	
	set_process(true)
	
	
	

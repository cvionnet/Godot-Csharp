extends KinematicBody2D


#*--    HEADER    ----------------------------------------------------------*//

export var speed:float = 50.0
export var speedRun:float = 20.0
export var inertia:float = 0.15	   # proche 0.0 = déplacement "lourd"  /  proche 1.0 = déplacement aérien

signal display_key(key)

var direction:Vector2 = Vector2.ZERO
var velocity:Vector2 = Vector2.ZERO

var run:Vector2 = Vector2(1.0,1.0)

#*--------------------------------------------------------------------------*//
#*--    GODOT METHODS    ---------------------------------------------------*//

# Not synced with physics. Execution done after the physics step
func _process(delta):
	ReadInputs()


# For processes that must happen before each physics step, such as controlling a character
func _physics_process(delta):
	direction = Vector2(
		Input.get_action_strength("L_right") - Input.get_action_strength("L_left"),
		Input.get_action_strength("L_down") - Input.get_action_strength("L_up")
	)
	direction = direction.normalized()
	
	# Si le joueur execute un boost
	if Input.is_action_just_pressed("button_A"):
		run = Vector2(speedRun, speedRun)
	# Si le joueur court
	elif Input.is_action_pressed("button_B"):
		run = Vector2(speedRun, speedRun)/9.0
	else:
		run = Vector2(1.0,1.0)

	# Interprétation lineaire entre la dernière vélocité connue et la nouvelle pour rendre le déplacement plus "smooth"
	velocity = lerp(velocity, direction * speed * run, inertia)

	move_and_slide(velocity)


#*--------------------------------------------------------------------------*//
#*--    SIGNAL CALLBACKS    ------------------------------------------------*//


#*--------------------------------------------------------------------------*//
#*--    USER METHODS    ----------------------------------------------------*//

func ReadInputs() -> void:
	var action:String = ""
	
	if Input.is_action_just_pressed("button_A"):
		action = "Button A"
	elif Input.is_action_just_pressed("button_B"):
		action = "Button B"
	elif Input.is_action_just_pressed("button_X"):
		action = "Button X"
	elif Input.is_action_just_pressed("button_Y"):
		action = "Button Y"
	elif Input.is_action_just_pressed("pad_up"):
		action = "Pad UP"
	elif Input.is_action_just_pressed("pad_down"):
		action = "Pad DOWN"
	elif Input.is_action_just_pressed("pad_left"):
		action = "Pad LEFT"
	elif Input.is_action_just_pressed("pad_right"):
		action = "Pad RIGHT"
	elif Input.is_action_just_pressed("trigger_LB"):
		action = "trigger LB"
	elif Input.is_action_just_pressed("trigger_RB"):
		action = "trigger RB"
	elif Input.is_action_just_pressed("trigger_LT"):
		action = "trigger LT"
	elif Input.is_action_just_pressed("trigger_RT"):
		action = "trigger RT"
	elif Input.is_action_just_pressed("R_up"):
		action = "Pad R UP"
	elif Input.is_action_just_pressed("R_down"):
		action = "Pad R DOWN"
	elif Input.is_action_just_pressed("R_left"):
		action = "Pad R LEFT"
	elif Input.is_action_just_pressed("R_right"):
		action = "Pad R RIGHT"
	elif Input.is_action_just_pressed("start"):
		action = "start"
	elif Input.is_action_just_pressed("back"):
		action = "back"
	elif Input.is_action_just_pressed("L_up"):
		action = "Pad L UP"
	elif Input.is_action_just_pressed("L_down"):
		action = "Pad L DOWN"
	elif Input.is_action_just_pressed("L_left"):
		action = "Pad L LEFT"
	elif Input.is_action_just_pressed("L_right"):
		action = "Pad L RIGHT"

	if(action != ""):
		emit_signal("display_key", action)


#*--------------------------------------------------------------------------*//

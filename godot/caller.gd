extends Node3D

@export var receiver_ref: Receiver
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	receiver_ref.on_call()
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

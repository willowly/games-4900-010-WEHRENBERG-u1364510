extends Area3D


func _ready() -> void:
	body_entered.connect(_on_body_entered)

func _on_body_entered(body: Node) -> void:
	print("Coin collected!")
	queue_free() # delete

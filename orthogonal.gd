extends Camera3D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
    pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
    var p = get_node("%Perspective")
    position = p.position
    rotation = p.rotation
    var s = get_node("%Size")
    size = s.value
    pass

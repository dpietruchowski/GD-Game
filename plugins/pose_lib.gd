
tool
extends EditorPlugin

func _enter_tree():
	add_custom_type("Pose", "Node", preload("pose_node.gd"))

func _exit_tree():
	remove_custom_type("BoneGizmo")

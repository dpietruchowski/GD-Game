tool
extends EditorPlugin

var inspector
var eds = get_editor_interface().get_selection()

func _enter_tree():
	eds.connect("selection_changed", self, "_on_selection_changed")
	add_custom_type("Bone", "Spatial", preload("bone.gd"), preload("bone.png"))
	add_custom_type("Pose", "Spatial", preload("pose.gd"), preload("pose.png"))
	inspector = preload("res://addons/poselib/inspector.gd").new()
	add_inspector_plugin(inspector)

func _exit_tree():
	remove_custom_type("Bone")
	remove_custom_type("Pose")
	remove_inspector_plugin(inspector)
	
func _on_selection_changed():
	var selected = eds.get_selected_nodes() 
	if not selected.empty():
		var bone = selected[0]
		if not bone or not bone.has_method("_on_selected"):
			return
		bone._on_selected()

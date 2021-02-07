tool
extends EditorInspectorPlugin
class_name CustomInspectorPlugin

func can_handle(object):
	return object is Pose or object is Bone
	
func parse_begin(object):
	if object.has_method("_parse_begin"):
		object._parse_begin(self)
		
func parse_property(object, type, path, hint, hint_text, usage):
	pass
		
func add_label(text):
	var label = Label.new()
	label.text = text
	#label.align = Label.ALIGN_CENTER
	add_custom_control(label)
	
func add_line_edit(text, handle, callback):
	var line_edit = LineEdit.new()
	line_edit.text = text
	line_edit.connect("text_changed", handle, callback)
	add_custom_control(line_edit)
	
		
func add_button(text, handle, callback):
	var button = Button.new()
	button.text = text
	button.connect("pressed", handle, callback)
	add_custom_control(button)

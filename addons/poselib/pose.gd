tool
extends Spatial
class_name Pose

export(NodePath) var skeleton_path

var add_pose_name:String = "None"
var current_pose_idx:int
var skel:Skeleton
var pose_lib:Dictionary
var combo_box:OptionButton

func _get(property):
	if property == "pose_lib_serialized":
		return self.pose_lib
		
func _set(property, value):
	if property == "pose_lib_serialized":
		self.pose_lib = value
		return true

func _get_property_list():
	if not Engine.editor_hint:
		return
	var properties = []
	properties.append({
			name = "pose_lib_serialized",
			type = TYPE_DICTIONARY,
			usage = PROPERTY_USAGE_STORAGE
	})
	return properties

func _parse_begin(p_inspector):
	p_inspector.add_label("Pose Library")
	p_inspector.add_button("Attach skeleton", self, "attach_skeleton")
	#p_inspector.add_button("Create bones", self, "_on_create_bones")
	#p_inspector.add_button("Save pose", self, "_on_save_pose")
	p_inspector.add_line_edit("None", self, "_on_line_edit_changed")
	p_inspector.add_button("Save pose", self, "_on_add_pose")
	p_inspector.add_button("Delete pose", self, "_on_delete_pose")
	combo_box = OptionButton.new()
	combo_box.connect("item_selected", self, "_on_item_selected")
	fill_combo_box()
	p_inspector.add_custom_control(combo_box)
	
func _on_line_edit_changed(text):
	add_pose_name = text
	
func _on_item_selected(item):
	current_pose_idx = item
	var text = combo_box.get_item_text(current_pose_idx)
	apply_pose(text)
	
func _on_save_pose():
	var text = combo_box.get_item_text(current_pose_idx)
	save_pose(text)

func _on_create_bones():
	if not skel:
		print("Skeleton not attached")
		return false
	
	var bones = {}
	for i in range(skel.get_bone_count()):
		var text = skel.get_bone_name(i)
		var bone = Bone.new()
		bone.name = text
		bone._bone_name = text
		bones[i] = bone
		
	for key in bones.keys():
		var parent = skel.get_bone_parent(key)
		if parent in bones:
			bones[parent].add_child(bones[key])
		else:
			add_child(bones[key])
	
	
func _on_add_pose():
#	if self.add_pose_name in self.pose_lib:
#		print("Pose ", self.add_pose_name, " already exist")
#		return
	if save_pose(self.add_pose_name):
		combo_box.add_item(self.add_pose_name)
		
	
func _on_override_pose():
	var text = combo_box.get_item_text(current_pose_idx)
	save_pose(text)

func _on_delete_pose():
	if combo_box.get_item_count() <= 0:
		return
	var text = combo_box.get_item_text(current_pose_idx)
	pose_lib.erase(text)
	combo_box.remove_item(current_pose_idx)
	if combo_box.get_item_count() <= 0:
		combo_box.clear()

func _ready():
	attach_skeleton()
	if not Engine.editor_hint:
		return
	print (self.pose_lib_serialized)
	
func _enter_tree():
	attach_skeleton()
		
func _on_selected():
	pass
	
func fill_combo_box():
	combo_box.clear()
	for pose_name in pose_lib.keys():
		combo_box.add_item(pose_name)

func apply_pose(pose_name):
	if not skel:
		return
	if not pose_name in pose_lib:
		return
	var pose = pose_lib[pose_name]
	for i in range(skel.get_bone_count()):
		if i in pose:
			skel.set_bone_rest(i, pose[i])
	print("Pose applied")
	
func save_pose(pose_name):
	if not skel:
		print("Skeleton not attached")
		return false
	var pose = {}
	if pose_name in pose_lib:
		pose = pose_lib[pose_name]
	for i in range(skel.get_bone_count()):
		pose[i] = skel.get_bone_rest(i)
	pose_lib[pose_name] = pose
	print("Pose saved")
	return true
		
func attach_skeleton():
	var node = get_parent()
	while node and not node as Skeleton:
		node = node.get_parent()
	self.skel = node as Skeleton
	if not self.skel:
		skeleton_path = NodePath()
		return
	skeleton_path = self.skel.get_path()
	
func get_current_pose():
	var pose = {}
	for i in range(skel.get_bone_count()):
		pose[i] = skel.get_bone_rest(i)
	return pose
	
func get_pose(pose_name):
	if not pose_name in self.pose_lib:
		return {}
	return self.pose_lib[pose_name] 
		
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			
			

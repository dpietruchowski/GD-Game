tool
extends Spatial
class_name Bone

export var moj = 1
export(NodePath) var skeleton_path
#export(NodePath) var skeleton_path setget _on_set_skeleton
export var bone_index = -1
var _bone_name:String

var skel

func _get(property):
	if property == "bone_name":
		return self._bone_name
		
func _set(property, value):
	if property == "bone_name":
		self._bone_name = value
		set_bone_name(value)
		return true

func _get_property_list():
	var properties = []
	properties.append({
			name = "bone_name",
			type = TYPE_STRING,
			hint = PROPERTY_HINT_ENUM,
			hint_string = get_bones_hint(),
			usage = PROPERTY_USAGE_SCRIPT_VARIABLE | PROPERTY_USAGE_NETWORK | PROPERTY_USAGE_EDITOR | PROPERTY_USAGE_STORAGE
	})
	return properties


func _ready():
	if Engine.editor_hint:
		attach_skeleton()
		set_notify_transform(true)

func _notification(NOTIFICATION_DRAW):
	if Engine.editor_hint:
		if NOTIFICATION_DRAW == NOTIFICATION_TRANSFORM_CHANGED:
			set_bone_pose()
	
func _on_selected():
	apply_bone_transform()

func _enter_tree():
	if Engine.editor_hint:
		attach_skeleton()
		apply_bone_transform()
	
func _on_set_skeleton(node_path):
	if not Engine.editor_hint:
		return
	if node_path:
		skel = get_node(node_path) as Skeleton
		if skel:
			#skeleton_path = node_path
			apply_bone_transform()
		else:
			print("This is not skeleton.")
			
func default_transform():
	self.transform = Transform()
	
func set_bone_name(value):
	if not skel:
		return false
	var idx = skel.find_bone(value)
	if idx < 0:
		return false
	if value != "":
		self.name = value
	apply_bone_transform()
	return false

func apply_bone_transform():
	if not Engine.editor_hint:
		default_transform()
		return
	var bone_idx = get_bone_idx()
	if bone_idx < 0:
		default_transform()
		return
	var parent = skel.get_bone_parent(bone_idx)
	var parent_bone_transform = skel.get_bone_global_pose(parent)
	var bone_pose = skel.get_bone_rest(bone_idx)
	var bone_transform = skel.get_bone_global_pose(bone_idx)
	self.transform.basis = bone_pose.basis
	self.transform.origin = parent_bone_transform.origin + parent_bone_transform.basis.xform(bone_pose.origin)
	
func set_bone_pose():
	++moj
	if not Engine.editor_hint:
		return
	var bone_idx = get_bone_idx()
	if bone_idx < 0:
		return
	var parent_idx = skel.get_bone_parent(bone_idx)
	var parent_bone_transform = skel.get_bone_global_pose(parent_idx)
	var bone_rest = Transform()
	bone_rest.basis = self.transform.basis
	bone_rest.origin = parent_bone_transform.basis.xform_inv(self.transform.origin - parent_bone_transform.origin)
	skel.set_bone_rest(bone_idx, bone_rest)
	
func get_bone_idx():
	if not Engine.editor_hint:
		return
	if not skel:
		attach_skeleton()
		if not skel:
			print("Skeleton not found")
			return -1
	var bone_idx = skel.find_bone(self._bone_name)
	if bone_idx < 0:
		print("Bone ", self._bone_name, " not found.")
	bone_index = bone_idx
	return bone_idx
	
func attach_skeleton():
	var node = get_parent()
	while node and not node as Skeleton:
		node = node.get_parent()
	self.skel = node as Skeleton
	if not self.skel:
		return
	skeleton_path = self.skel.get_path()
	
func get_bones_hint():
	if not self.skel:
		return "None"
	var hint = ""
	for i in range(self.skel.get_bone_count()):
		hint += ","
		hint += self.skel.get_bone_name(i)
	return hint
	

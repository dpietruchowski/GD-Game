using Godot;
using System;

public class Camera : Godot.Camera
{
	float distance = 4.0f;	
	float height = 2.0f;
	
	public override void _Ready()
	{
		SetAsToplevel(true);
	}
	
	public override void _PhysicsProcess(float delta)
	{
		var target = GetParent<Spatial>().GlobalTransform.origin;
		var pos = GlobalTransform.origin;
		
		var offset = pos - target;
		offset = offset.Normalized() * distance;
		offset.y = height;
		
		pos = target + offset;
		LookAtFromPosition(pos, target, new Vector3(0, 1, 0));
	}
}

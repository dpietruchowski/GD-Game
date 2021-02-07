using Godot;
using System;
using static SphereCoords;

public class PlayerCamera : Godot.Camera
{
	Vector3 spherePosition = new Vector3(3, 1, 1);
	Vector2 mousePosition = new Vector2();
	float beginZ = -3.14f;
	
	public override void _Ready()
	{
		
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton) {
			if (eventMouseButton.IsPressed()) {
				mousePosition = eventMouseButton.Position;
				var parent = GetParent<Spatial>();
				var pos = ConvertToSphere(parent.ToLocal(GlobalTransform.origin));
				beginZ = pos.z;
			}
		} else if (@event is InputEventMouseMotion eventMouseMotion && Input.IsActionPressed("move_camera")) {
			var dir = mousePosition - eventMouseMotion.Position;
			spherePosition.z = Map(dir.x, -GetViewport().Size.x, GetViewport().Size.x, beginZ-3.14f, beginZ+3.14f);
		}
	}
	
	public override void _PhysicsProcess(float delta)
	{
		var parent = GetParent<Spatial>();
		LookAtFromPosition(parent.ToGlobal(ConvertToCartesian(spherePosition)), 
						   parent.ToGlobal(new Vector3(0,1,0)), 
						   new Vector3(0,1,0));
	}
}

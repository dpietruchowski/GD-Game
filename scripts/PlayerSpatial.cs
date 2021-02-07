using Godot;
using System;
using static SphereCoords;

public class PlayerSpatial : Spatial
{
	float speed = 4;
	float acceleration = 2;
	Vector3 gravity = new Vector3(0, -10, 0);
	Vector3 velocity = new Vector3();
	KinematicBody body;
	Camera camera;
	

	public override void _Ready()
	{
		body = GetNode<KinematicBody>("Body");
		camera = GetNode<Camera>("Camera");
		camera.MakeCurrent();
	}

	public override void _PhysicsProcess(float delta)
	{
		var base_direction = camera.GlobalTransform.basis;
		var direction = delta * gravity;
		if(Input.IsActionPressed("turn_left")) {
			RotateY(-0.1f);
		}
		if(Input.IsActionPressed("turn_right")) {
			RotateY(0.1f);
		}
		if(Input.IsActionPressed("move_forward")) {
			direction += base_direction.z;
		}
		if(Input.IsActionPressed("move_backward")) {
			direction -= base_direction.z;
		}
		if(Input.IsActionPressed("move_left")) {
			direction += base_direction.x;
		}
		if(Input.IsActionPressed("move_right")) {
			direction -= base_direction.x;
		}
		GD.Print(direction);
		
		var lookDirection = new Vector3(direction.x, 0, direction.y).Normalized();
		body.LookAt(lookDirection, new Vector3(0, 1, 0));
		
		direction += delta * gravity;
		direction = direction.Normalized();
		
		velocity = velocity.LinearInterpolate(direction*speed, acceleration*delta);
		if(Input.IsActionPressed("jump")) {
			velocity.y = 3;
		}
		
		velocity = body.MoveAndSlide(velocity);
		
	}
}

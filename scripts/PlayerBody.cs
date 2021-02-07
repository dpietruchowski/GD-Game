using Godot;
using System;
using static SphereCoords;

public class PlayerBody : KinematicBody
{
	Vector2 mousePosition = new Vector2();
	Camera camera;
	
	double counter;
	Skeleton model;
	Skeleton pose0;
	Skeleton pose1;
	float dist = 0;
	
	public void InterpolatePose(float weight) {
		for(int i = 0; i < model.GetBoneCount(); ++i) {
			Transform beginTransform = pose0.GetBoneRest(i);
			Transform endTransform = pose1.GetBoneRest(i);
			Transform t = beginTransform.InterpolateWith(endTransform, (float)(1 + Math.Sin((double)weight))/2);
			model.SetBoneRest(i, t);
		}
	}

	public override void _Ready()
	{
		//camera = GetNode<Camera>("Camera");
		//camera.MakeCurrent();
		
		dist = GlobalTransform.origin.x;
		
		model = GetNode<Skeleton>("Model");
		pose0 = GetNode<Skeleton>("Pose0");
		pose1 = GetNode<Skeleton>("Pose1");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton) {
			if (eventMouseButton.IsPressed()) {
				mousePosition = eventMouseButton.Position;
			} else {
				counter = 0;
			}
		} else if (@event is InputEventMouseMotion eventMouseMotion && Input.IsActionPressed("move_hand")) {
			var dir = mousePosition - eventMouseMotion.Position;
			var deg = Map(dir.x, 0, GetViewport().Size.x, 0, 6.28f);
		}
	}

	public override void _PhysicsProcess(float delta)
	{		
		counter += delta;
		InterpolatePose(dist - GlobalTransform.origin.y);
	}
}

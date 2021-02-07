using Godot;
using System;
using static SphereCoords;

enum State
{
	Standing,
	Walking,
	Running,
	Flying
}

public class Player : KinematicBody
{
	Vector2 mousePosition = new Vector2();

	float speed = 4;
	float acceleration = 2;
	Vector3 gravity = new Vector3(0, -15, 0);
	Vector3 velocity = new Vector3();
	Camera camera;
	
	Model model;
	
	float distance = 0;
	State prevState = State.Standing;
	float transitionWeight = 0.0f;
	String flyingPose = "Fly";
	String standingPose = "T-Pose";
	String[] walkingPoses = {"Walk1", "Walk2", "Walk3" ,"Walk4"};
	String[] runingPoses = {"Run1", "Run2", "Run3"};
	String crouchPose = "Crouch2";
	
	public void InterpolateWalkPose(float weight) {
		float val = Math.Abs((weight*2.1f) % 4 - 0.01f);
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 3 ? 0 : begin + 1;
		//GD.Print(walkingPoses[begin], " ", walkingPoses[end], " ", val, " ", (int)val);
		model.SetInterpolatedPose(walkingPoses[begin], walkingPoses[end], realWeight);
	}
	public void InterpolateRunPose(float weight) {
		float val = Math.Abs((weight*1.0f) % 3 - 0.01f);
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 2 ? 0 : begin + 1;
		model.SetInterpolatedPose(runingPoses[begin], runingPoses[end], realWeight);
	}

	public override void _Ready()
	{
		camera = GetNode<Camera>("CameraTarget/Camera");
		camera.MakeCurrent();
		
		model = GetNode<Model>("Model");
		model.ApplyPose("Run1");
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton) {
			if (eventMouseButton.IsPressed()) {
				mousePosition = eventMouseButton.Position;
			} else {
			}
		} else if (@event is InputEventMouseMotion eventMouseMotion && Input.IsActionPressed("move_hand")) {
			var dir = mousePosition - eventMouseMotion.Position;
			var deg = Map(dir.x, 0, GetViewport().Size.x, 0, 6.28f);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		var baseDirection = camera.GlobalTransform.basis;
		var direction = new Vector3();
		if(Input.IsActionPressed("turn_left")) {
			RotateY(-0.1f);
		}
		if(Input.IsActionPressed("turn_right")) {
			RotateY(0.1f);
		}
		var z = baseDirection.z;
		z.y = 0;
		if(Input.IsActionPressed("move_forward")) {
			direction -= z;
		}
		if(Input.IsActionPressed("move_backward")) {
			direction += z;
		}
		if(Input.IsActionPressed("move_left")) {
			direction -= baseDirection.x;
		}
		if(Input.IsActionPressed("move_right")) {
			direction += baseDirection.x;
		}
			
		direction = direction.Normalized();
		direction += delta * gravity;
		direction = direction.Normalized();
		
		velocity = velocity.LinearInterpolate(direction*speed, acceleration*delta);
		var velocity2d = new Vector2(velocity.x, velocity.z);
		if (velocity2d.Length() > 0.1) {
			var angle = Math.Atan2((double)velocity2d.x, (double)velocity2d.y);
			var rotation = Rotation;
			rotation.y = (float)angle;
			Rotation = rotation;
		}
		if(Input.IsActionPressed("jump")) {
			velocity.y += (gravity.y*-2)*delta;
			GD.Print(velocity.y);
		}
		
		velocity = MoveAndSlide(velocity);
		var vLength = velocity2d.Length();
		distance += vLength * delta;
		//GD.Print(velocity2d.Length());
		if (velocity.y > 0.5) {
			float w = Math.Abs(velocity.y)/2;
			model.SetInterpolatedPose(flyingPose, w < 1 ? w : 1 );
			prevState = State.Flying;
			transitionWeight = 0;
		} else if (vLength < 0.5f) {
			if(Input.IsActionPressed("crouch")) {
				model.SetInterpolatedPose(crouchPose, transitionWeight < 1 ? transitionWeight : 1);
				transitionWeight += delta*4;
			} else {
				model.SetInterpolatedPose(standingPose, (0.5f - vLength)*2);
				prevState = State.Standing;
				transitionWeight = 0;
			}
		} else if (vLength > 3.0f) {
			if (prevState != State.Running && transitionWeight < 1) {
				model.SetInterpolatedPose(runingPoses[0], transitionWeight);
				transitionWeight += vLength * delta;
				GD.Print("Trans W->R", transitionWeight);
			} else {
				if(Input.IsActionPressed("crouch")) {
					model.SetInterpolatedPose(crouchPose, transitionWeight < 1 ? transitionWeight : 1);
					transitionWeight += delta;
				} else {
					InterpolateRunPose(distance);
					transitionWeight = 0;
				}
				prevState = State.Running;
			}
		} else {
			if (prevState != State.Walking && transitionWeight < 1) {
				model.SetInterpolatedPose(walkingPoses[0], transitionWeight);
				transitionWeight += vLength * delta * 2;
				GD.Print("Trans R->W", transitionWeight);
			} else {
				InterpolateWalkPose(distance);
				prevState = State.Walking;
				transitionWeight = 0;
			}
		}
	}
}

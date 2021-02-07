using Godot;
using System;
using System.Collections.Generic;
using static SphereCoords;

public class Player : KinematicBody, IPlayer
{
	Vector2 mousePosition = new Vector2();

	float speed = 4;
	float acceleration = 2;
	Vector3 gravity = new Vector3(0, -15, 0);
	Vector3 velocity = new Vector3();
	Camera camera;
	
	Model model;
	
	private State state;
	private StateContext prevContext;
	private Dictionary<States, State> states = new Dictionary<States, State>();
	
	public Player()
	{
		states.Add(States.Standing, new StateStand(this));
		states.Add(States.Walking, new StateWalk(this));
		states.Add(States.Running, new StateRun(this));
		states.Add(States.Crouching, new StateCrouch(this));
		states.Add(States.Flying, new StateFly(this));
		states.Add(States.Rolling, new StateRoll(this));
		state = states[States.Standing];
	}
	
	public void SetState(States newState)
	{	
		var nState = states[States.Standing];
		if (states.ContainsKey(newState)) {
			nState = states[newState];
		}
		if (nState == state) {
			return;
		}
		state = nState;
		state.OnStateSet();
		SaveCurrentPose();
	}

	public void InterpolatePose(string newPose, float weight)
	{
		GD.Print("Interpolate pose ", newPose, "-", weight);
		model.SetInterpolatedPose(newPose, weight);
	}

	public void InterpolatePose(string beginPose, string endPose, float weight)
	{
		GD.Print("Interpolate pose ", beginPose, " ", endPose, "-", weight);
		model.SetInterpolatedPose(beginPose, endPose, weight);
	}

	public void SaveCurrentPose()
	{
		model.SaveCurrentPose();
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
		} else if (@event.IsActionPressed("crouch"))
		{
			SetState(States.Crouching);
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
		}
		
		velocity = MoveAndSlide(velocity);
		var vLength = velocity2d.Length();
		//distance += vLength * delta;
		
		
		if (GlobalTransform.origin.y > 0.5f) {
			SetState(States.Flying);
		}
		StateContext context = new StateContext(velocity2d, GlobalTransform.origin.y, delta);
		state.Update(context, prevContext);
		prevContext = context;
	}
}

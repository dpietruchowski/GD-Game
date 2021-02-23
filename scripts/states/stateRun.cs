using Godot;
using System;
using static Perlin;

public class StateRun: State
{    
	float distance = 0;
	const float minVelocity = 4.0f;
	bool transition = false;
	
	public StateRun(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		if (!context.IsOnGround) {
			player.SetState(States.Jumping);
			return;
		}
		var length = context.Velocity.Length();
		if (length < minVelocity) {
			player.SetState(States.Walking);
			return;
		}
		distance += length * context.Delta;
		float weight = CountWeight(distance);
		if (transition)
			weight *= 4;
		player.TransitionPose("Run", weight % 1, transition);
		if (weight >= 1.0f && transition) {
			transition = false;
			distance = (weight % 1) / 0.3f;
		}
	}

	public override void OnStateSet() 
	{
		distance = 0;
		transition = true;
	}

	float CountWeight(float x)
	{
		return (float)Math.Abs(0.3f*x);// + (float)Math.Sin(2*x);
	}
}

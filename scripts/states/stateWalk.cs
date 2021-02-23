using Godot;
using System;

public class StateWalk: State
{    
	float distance = 0;
	const float maxVelocity = 4.0f;
	const float minVelocity = 0.1f;
	
	public StateWalk(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		var length = context.Velocity.Length();
		if (!context.IsOnGround) {
			player.SetState(States.Jumping);
			return;
		}
		if (length > maxVelocity) {
			player.SetState(States.Running);
			return;
		} else if (length < minVelocity) {
			player.SetState(States.Standing);
			return;
		}
		
		float weight = CountWeight(distance);
		player.TransitionPose("Walk", weight % 1);
		distance += length * context.Delta;
	}

	public override void OnStateSet() 
	{
		distance = 0;
	}
	
	float CountWeight(float weight)
	{
		return Math.Abs(weight*0.5f);
	}
}

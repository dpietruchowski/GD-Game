using Godot;
using System;

public class StateStand: State
{
	const float maxVelocity = 0.05f;
	float transitionWeight = 0.0f;
	float prevVelocity = 0.0f;
	
	public StateStand(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		var length = context.Velocity.Length();
		var prevLength = prevContext.Velocity.Length();
		if (length > maxVelocity && prevLength < length) {
			player.SetState(States.Walking);
			return;
		}
		if (prevLength < length) {
			transitionWeight += context.Delta * 15;
		} else {
			transitionWeight += context.Delta * 4;
		}
		player.InterpolatePose("T-Pose", transitionWeight <= 1 ? transitionWeight : 1);
	}
	
	public override void OnStateSet() 
	{
		transitionWeight = 0.0f;
	}
}

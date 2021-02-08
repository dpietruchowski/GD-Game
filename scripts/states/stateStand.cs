using Godot;
using System;

public class StateStand: State
{
	const float maxVelocity = 0.05f;
	float transitionWeight = 0.0f;
	float prevVelocity = 0.0f;
	string[] poses = {"Stand1", "Stand0", "Stand2"}; 
	
	public StateStand(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		if (!context.IsOnGround) {
			player.SetState(States.Jumping);
			return;
		}
		var length = context.Velocity.Length();
		var prevLength = prevContext.Velocity.Length();
		if (length > maxVelocity && (prevLength < length || context.Height < 0.5f)) {
			player.SetState(States.Walking);
			return;
		}
		if (prevLength < length) {
			transitionWeight += context.Delta * 15;
		} else {
			transitionWeight += context.Delta * 4;
		}
			player.InterpolatePose(poses[1], transitionWeight <= 1 ? transitionWeight : 1);
	}
	
	public override void OnStateSet() 
	{
		transitionWeight = 0.0f;
	}
	
	void InterpolatePose(float weight)
	{
		int s = 3;
		float val = CountWeight(weight) % s;
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= s-1 ? 0 : begin + 1;
		player.InterpolatePose(poses[begin], poses[end], realWeight);
	}
	
	float CountWeight(float weight)
	{
		return (float)Math.Abs((1 + Math.Sin(weight/10))*1.5);
	}
}

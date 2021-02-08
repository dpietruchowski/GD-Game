using Godot;
using System;

public class StateRoll: State
{    
	float distance = 0;
	bool transition = true;
	const float minVelocity = 0.3f;
	string[] poses = {"Roll1", "Roll2", "Roll3", "Roll4"}; 
	
	public StateRoll(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		var length = context.Velocity.Length();
		if (length < minVelocity) {
			player.SetState(States.Crouching);
			return;
		}
		float weight = distance;
		if (transition) {
			float realWeight = CountWeight(weight);
			player.InterpolatePose(poses[0], realWeight);
			if (realWeight >= 1.0f) {
				transition = false;
				distance = 0;
			}
		} else {
			InterpolatePose(weight);
		}
		distance += length * context.Delta;
	}

	public override void OnStateSet() 
	{
		distance = 0;
		transition = true;
	}

	void InterpolatePose(float weight)
	{
		float val = CountWeight(weight) % 4;
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 3 ? 0 : begin + 1;
		player.InterpolatePose(poses[begin], poses[end], realWeight);
	}

	float CountWeight(float weight)
	{
		return Math.Abs(weight*2.1f);
	}
}

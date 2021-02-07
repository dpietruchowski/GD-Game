using Godot;
using System;

public class StateWalk: State
{    
	float distance = 0;
	bool transition = true;
	const float maxVelocity = 3.0f;
	const float minVelocity = 0.1f;
	string[] poses = {"Walk1", "Walk2", "Walk3" ,"Walk4"}; 
	
	public StateWalk(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		var length = context.Velocity.Length();
		if (length > maxVelocity) {
			player.SetState(States.Running);
			return;
		} else if (length < minVelocity) {
			player.SetState(States.Standing);
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

using Godot;
using System;

public class StateRun: State
{    
	float distance = 0;
	bool transition = true;
	const float minVelocity = 3.0f;
	string[] poses = {"Run1", "Run2", "Run3"}; 
	
	public StateRun(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		var length = context.Velocity.Length();
		if (length < minVelocity) {
			player.SetState(States.Walking);
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
		float val = CountWeight(weight) % 3;
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 2 ? 0 : begin + 1;
		player.InterpolatePose(poses[begin], poses[end], realWeight);
	}

	float CountWeight(float weight)
	{
		return Math.Abs(weight*1.0f);
	}
}

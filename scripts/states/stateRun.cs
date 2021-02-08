using Godot;
using System;
using static Perlin;

public class StateRun: State
{    
	float distance = 0;
	bool transition = true;
	const float minVelocity = 4.0f;
	string[] poses = {"Run1", "Run2", "Run3", "Run4"}; 
	
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

	float CountWeight(float x)
	{
		return (float)Math.Abs(1.5f*x);// + (float)Math.Sin(2*x);
	}
}

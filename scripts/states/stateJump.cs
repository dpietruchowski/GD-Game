using Godot;
using System;

public class StateJump: State
{
	const float minHeight = 1.3f;
	float height = 0.0f;
	bool transition = true;
	string[] poses = {"Jump1"}; 
	
	public StateJump(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		if (context.IsOnGround) {
			player.SetState(States.Standing);
			return;
		}
		var vel3 = new Vector3(context.Velocity.x, context.Height, context.Velocity.y);
		height += vel3.Length() * context.Delta;
		/*if (transition) {
			float realWeight = CountWeight(height);
			player.InterpolatePose(poses[0], realWeight);
			if (realWeight >= 1.0f) {
				transition = false;
				height = 0;
			}
		} else {
		}*/
		if (context.Height > minHeight) {
			float realWeight = CountWeight(height);
			player.InterpolatePose(poses[0], realWeight < 1 ? realWeight : 0);
		} else {
			player.InterpolatePose("Stand0", minHeight - context.Height);
		}
	}
	
	public override void OnStateSet() 
	{
		height = 0;
		transition = true;
	}

	void InterpolatePose(float weight)
	{
		float val = CountWeight(weight) % 2;
		float realWeight = val - (int)val;
		
		int begin = (int)val;
		int end = begin >= 1 ? 0 : begin + 1;
		player.InterpolatePose(poses[begin], poses[end], realWeight);
	}

	float CountWeight(float weight)
	{
		return Math.Abs(weight*0.7f);
	}
}

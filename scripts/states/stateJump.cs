using Godot;

public class StateJump: State
{
	const float minHeight = 0.5f;
	float transitionWeight = 0.0f;
	
	public StateJump(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		var length = context.Velocity.Length();
		if (context.Height < minHeight) {
			player.SetState(States.Standing);
			return;
		}
		transitionWeight += context.Delta * 4;
		player.InterpolatePose("Fly", transitionWeight);
	}
	
	public override void OnStateSet() 
	{
		transitionWeight = 0.0f;
	}
}

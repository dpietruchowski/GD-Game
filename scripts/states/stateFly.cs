using Godot;

public class StateFly: State
{
	const float minHeight = 0.5f;
	float transitionWeight = 0.0f;
	
	public StateFly(IPlayer _player): base(_player)
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
		player.InterpolatePose("Fly", transitionWeight <= 1 ? transitionWeight : 1);
	}
	
	public override void OnStateSet() 
	{
		transitionWeight = 0.0f;
	}
}

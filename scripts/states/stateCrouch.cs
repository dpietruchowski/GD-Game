using Godot;

public class StateCrouch: State
{
	float transitionWeight = 0.0f;
	const float maxVelocity = 0.1f;
	
	public StateCrouch(IPlayer _player): base(_player)
	{
	}
	
	public override void Update(StateContext context, StateContext prevContext)
	{
		if(!Input.IsActionPressed("crouch")) {
			player.SetState(States.Standing);
			return;
		}
		var length = context.Velocity.Length();
		var prevLength = prevContext.Velocity.Length();
		if (length > maxVelocity) {
			player.SetState(States.Rolling);
			return;
		}
		transitionWeight += context.Delta * 4;
		player.InterpolatePose("Crouch2", transitionWeight <= 1 ? transitionWeight : 1);
	}
	
	public override void OnStateSet() 
	{
		transitionWeight = 0.0f;
	}
}

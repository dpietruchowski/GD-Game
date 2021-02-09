using Godot;

public enum States  
{
	Standing,
	Walking,
	Running,
	Jumping,
	Crouching,
	Flying,
	Rolling
}

public struct StateContext
{
	public StateContext(Vector2 velocity, float height, bool onGround, float delta)
	{
		Velocity = velocity;
		Height = height;
		IsOnGround = onGround;
		Delta = delta;
	}

	public Vector2 Velocity { get; }
	public float Height { get; }
	public bool IsOnGround { get; }
	public float Delta { get; }
} 

public abstract class State
{
	public IPlayer player;

	public State(IPlayer _player) 
	{
		player = _player;
	}

	public abstract void Update(StateContext context, StateContext prevContext);
	public abstract void OnStateSet();
}

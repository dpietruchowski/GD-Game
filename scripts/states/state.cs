using Godot;

public struct StateContext
{
	public StateContext(Vector2 velocity, float height, float delta)
	{
		Velocity = velocity;
		Height = height;
		Delta = delta;
	}

	public Vector2 Velocity { get; }
	public float Height { get; }
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

using Godot;
using IPlayer;

public class State
{
    public IPlayer player;

    public State(IPlayer _player) 
    {
        player = _player;
    }

    public virtual void Update(Vector3 velocity);
    public virtual void OnStateSet();
}
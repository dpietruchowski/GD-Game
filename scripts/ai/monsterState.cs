
public enum MonsterStates
{
	Idle,
	Follow,
	Attack
}

public class MonsterContext
{
	public MonsterContext(float delta)
	{
		Delta = delta;
	}

	public float Delta { get; }
}

public abstract class MonsterState
{
	protected IMonster monster;
	
	public MonsterState(IMonster _monster) 
	{
		monster = _monster;
	}

	public abstract void Update(MonsterContext context);
	public abstract void OnStateSet();
}

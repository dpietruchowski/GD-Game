using Godot;

public class StateIdle: MonsterState
{
	
	public StateIdle(IMonster _monster): base(_monster)
	{
	}
	
	public override void Update(MonsterContext context)
	{
		if (monster.Target != null) {
			monster.SetState(MonsterStates.Attack);
		}
	}
	
	public override void OnStateSet() 
	{
	}
}

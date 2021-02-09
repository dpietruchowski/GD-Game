using Godot;

public class StateAttack: MonsterState
{
	
	public StateAttack(IMonster _monster): base(_monster)
	{
	}
	
	public override void Update(MonsterContext context)
	{
		monster.Shoot(context.Delta);
		if (monster.Target == null) {
			monster.SetState(MonsterStates.Idle);
		}
	}
	
	public override void OnStateSet() 
	{
	}
}

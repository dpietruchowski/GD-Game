
public interface IMonster
{
	void SetState(MonsterStates state);
	void Shoot(float delta);
	
	IDestroyable Target { get; }
}

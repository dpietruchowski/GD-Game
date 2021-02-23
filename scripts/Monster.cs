using Godot;
using System;
using System.Collections.Generic;

public class Monster : KinematicBody, IDestroyable, IMonster
{
	float speed = 6;
	float acceleration = 2;
	Vector3 gravity = new Vector3(0, -15, 0);
	Vector3 velocity = new Vector3();
	
	TextureProgress hpBar;
	float hp = 100;
	IDestroyable target;
	Vector3 attackFrom;
	
	Timer timer;
	RayCast rifleRay;
	
	MonsterState state;
	Dictionary<MonsterStates, MonsterState> states = new Dictionary<MonsterStates, MonsterState>();
	
	public IDestroyable Target { get { return target; } }
	
	public Monster()
	{
		Engine.TimeScale = 1.0f;
		states.Add(MonsterStates.Idle, new StateIdle(this));
		states.Add(MonsterStates.Attack, new StateAttack(this));
		state = states[MonsterStates.Idle];
	}
	
	public void SetState(MonsterStates newState)
	{	
		var nState = states[MonsterStates.Idle];
		if (states.ContainsKey(newState)) {
			nState = states[newState];
		}
		if (nState == state) {
			return;
		}
		state = nState;
		GD.Print("Monster Set state to " , newState);
		state.OnStateSet();
	}
	
	public void Shoot(float delta)
	{
		if (target == null) {
			return;
		}
		GD.Print(rifleRay.GetCollider());
		if (rifleRay.GetCollider() is IDestroyable) {
			var collider = (IDestroyable) rifleRay.GetCollider();
			if (collider == target)
				collider.TakeDamage(30 * delta, GlobalTransform.origin);
		}
	}
	
	public void TakeDamage(float dmg, Vector3 fromPos)
	{
		attackFrom = fromPos;
		hp -= dmg;
		hpBar.Value = hp;
		if (hp < 0) {
			QueueFree();
		}
	}
	
	public void OnBodyEntered(Node body)
	{
		if (body is Player) {
			target = (IDestroyable) body;
		}
	}
	
	public void OnBodyExited(Node body)
	{
		if (body == target) {
			target = null;
		}
	}
	
	public override void _Ready()
	{
		hpBar = GetNode<TextureProgress>("Viewport/TextureProgress");
		hpBar.MaxValue = 0;
		hpBar.MaxValue = hp;
		hpBar.Value = hp;
		var area = GetNode<Area>("Area");
		area.Connect("body_entered", this, nameof(OnBodyEntered));
		area.Connect("body_exited", this, nameof(OnBodyExited));
		timer = GetNode<Timer>("Timer");
		rifleRay = GetNode<RayCast>("RayCast");
		attackFrom = GlobalTransform.origin;
	}

	public override void _PhysicsProcess(float delta)
	{
		var baseDirection = GlobalTransform.basis;
		var direction = new Vector3();
		
		if (target is Spatial) {
			var targetDirection = GlobalTransform.origin - ((Spatial)target).GlobalTransform.origin;
			direction -= targetDirection;
		} else {
			var targetDirection = GlobalTransform.origin - attackFrom;
			direction -= targetDirection;
		}
		
		direction = direction.Normalized();
		
		velocity = velocity.LinearInterpolate(direction*speed, acceleration*delta);
		velocity += delta * gravity;
		
		var velocity2d = new Vector2(velocity.x, velocity.z);
		
		
		/*if (target is PhysicsBody) {
			LookAt(((PhysicsBody)target).GlobalTransform.origin, new Vector3(0, 1, 0));
		} else */
		if (velocity2d.Length() > 0.1) {
			var angle = Math.Atan2((double)velocity2d.x, (double)velocity2d.y);
			var rotation = Rotation;
			rotation.y = (float)angle;
			Rotation = rotation;
		}
		
		velocity = MoveAndSlide(velocity);
		
		velocity2d = new Vector2(velocity.x, velocity.z);
		if (velocity2d.Length() > 0.1) {
			if (!GetNode<AnimationPlayer>("monster/AnimationPlayer").IsPlaying())
				GetNode<AnimationPlayer>("monster/AnimationPlayer").Play("walk");
		}
		
		state.Update(new MonsterContext(delta));
	}
}

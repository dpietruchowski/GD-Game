using Godot;
using System;

public class Grenade : RigidBody
{
	public override void _Ready()
	{
		SetAsToplevel(true);
		var timer = GetNode<Timer>("Timer");
		timer.Connect("timeout", this, nameof(OnTimeout));
		timer.Start(2);
		var animation = GetNode<AnimationPlayer>("AnimationPlayer");
		animation.Connect("animation_finished", this, nameof(OnAnimationFinished));
	}
	
	void OnTimeout()
	{
		Bang();
	}
	
	void OnAnimationFinished(String name)
	{
		QueueFree();
	}
	
	void Bang()
	{
		var animation = GetNode<AnimationPlayer>("AnimationPlayer");
		animation.Play("Bang");
		var area = GetNode<Area>("Area");
		var bodies = area.GetOverlappingBodies();
		for (int i = 0; i < bodies.Count; ++i)
		{
			var body = bodies[i] as IDestroyable;
			if (body != null)
			{
				body.TakeDamage(30, GlobalTransform.origin);
			}
		}
	}
}

using Godot;
using System;

public interface IDestroyable
{
	void TakeDamage(float dmg, Vector3 fromPos);
}

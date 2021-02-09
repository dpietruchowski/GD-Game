using Godot;
using System;

public class World : Spatial
{
	Player player;
	TextureProgress hpBar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.Connect("HpChanged", this, nameof(OnPlayerHpChanged));
		hpBar = GetNode<TextureProgress>("Gui/TextureProgress");
		hpBar.MaxValue = 0;
		hpBar.MaxValue = player.MaxHp;
		hpBar.Value = player.Hp;
		GD.Print(hpBar.Value);
	}

	public void OnPlayerHpChanged(float hp)
	{
		hpBar.Value = hp;
	}
}

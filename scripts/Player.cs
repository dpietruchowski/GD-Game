using Godot;
using System;
using System.Collections.Generic;
using static SphereCoords;

public class Player : KinematicBody, IPlayer, IDestroyable
{	
	float maxHp = 100;
	float hp = 100;
	int ammo = 100;
	int grenadeAmount = 10;
	
	
	float speed = 6;
	float acceleration = 2;
	Vector3 gravity = new Vector3(0, -15, 0);
	Vector3 velocity = new Vector3();
	Camera camera;
	
	const float rayLength = 50;
	Vector2 mousePosition = new Vector2();
	
	Model model;
	Transform chestPose;
	RayCast rifleRay;
	
	private State state;
	private StateContext prevContext;
	private Dictionary<States, State> states = new Dictionary<States, State>();
	
	// Signals
	[Signal]
	public delegate void HpChanged(float hp);
	[Signal]
	public delegate void AmmoChanged(int ammo);
	[Signal]
	public delegate void GrenadeChanged(float grenade);
	
	// GetSet
	public float Hp { get => hp; }
	public float MaxHp { get => maxHp; }
	
	public Player()
	{
		Engine.TimeScale = 1.0f;
		states.Add(States.Standing, new StateStand(this));
		states.Add(States.Walking, new StateWalk(this));
		states.Add(States.Running, new StateRun(this));
		states.Add(States.Crouching, new StateCrouch(this));
		states.Add(States.Flying, new StateFly(this));
		states.Add(States.Rolling, new StateRoll(this));
		states.Add(States.Jumping, new StateJump(this));
		state = states[States.Standing];
	}
	
	public void SetState(States newState)
	{	
		var nState = states[States.Standing];
		if (states.ContainsKey(newState)) {
			nState = states[newState];
		}
		if (nState == state) {
			return;
		}
		state = nState;
		//GD.Print("Set state to " , newState);
		state.OnStateSet();
		SaveCurrentPose();
	}

	public void InterpolatePose(string newPose, float weight)
	{
		//GD.Print("Interpolate pose ", newPose, "-", weight);
		model.SetInterpolatedPose(newPose, weight);
	}

	public void InterpolatePose(string beginPose, string endPose, float weight)
	{
		//GD.Print("Interpolate pose ", beginPose, " ", endPose, "-", weight);
		model.SetInterpolatedPose(beginPose, endPose, weight);
	}
	
	public void TransitionPose(string poseName, float weight, bool transition = false)
	{
		model.TransitionPose(poseName, weight, transition);
	}

	public void SaveCurrentPose()
	{
		model.SaveCurrentPose();
	}
	
	public void TakeDamage(float dmg, Vector3 fromPos)
	{
		hp -= dmg;
		EmitSignal(nameof(HpChanged), hp);
		if (hp <= 0) {
		}
	}
	
	public bool IsOnGround()
	{
		return GetNode<RayCast>("RayCast").IsColliding();
	}
	
	void LookAtTarget(Vector2 mousePosition)
	{
		var source = camera.ProjectRayOrigin(mousePosition);
		var target = source + camera.ProjectRayNormal(mousePosition) * rayLength;
		var spaceState = GetWorld().DirectSpaceState;
		var result = spaceState.IntersectRay(source, target, new Godot.Collections.Array() { this.GetRid() });
		if (result.Contains("position")) {
			target = (Vector3)result["position"];
		}
		var gt = GetNode<Spatial>("targert").GlobalTransform;
		gt.origin = target;
		GetNode<Spatial>("targert").GlobalTransform = gt;
		
		var chest = GetNode<Bone>("Model/ChestBone");
		chest.MoveToBonePosition(model, "Chest");;
		chest.LookAt(target, new Vector3(0, 1, 0));
		var t = chest.GlobalTransform;
		t.basis.x = -t.basis.x;
		t.basis.z = -t.basis.z;
		chest.GlobalTransform = t;
		chestPose = chest.GetBoneRestTransform(model, "Chest");
		var idx = model.FindBone("Chest");
		//model.SetBoneRest(idx, chestPose);
		//model.BlockBone("Chest");
	}
	
	void Shoot(float delta)
	{
		model.SetInterpolatedPose("Aim", 1, new string[] {"RightUpperArm", "RightLowerArm", "RightHand",
				"LeftUpperArm", "LeftLowerArm", "LeftHand"});
		var idx = model.FindBone("Chest");
		model.SetBoneRest(idx, chestPose);
		if (ammo <= 0) {
			var timer = GetNode<Timer>("ReloadTimer");
			if (timer.IsStopped()) {
		 		GetNode<Timer>("ReloadTimer").Start(1.5f);
			}
			return;
		}
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Fire");
		Random r = new Random();
		if (rifleRay.GetCollider() is IDestroyable) {
			var collider = (IDestroyable) rifleRay.GetCollider();
			collider.TakeDamage(60 * delta + r.Next(-10, 10)/20, GlobalTransform.origin);
		}
		ammo -= 1;
		EmitSignal(nameof(AmmoChanged), ammo);
	}
	
	void ThrowGrenade()
	{
		if (grenadeAmount <= 0)
			return;
		var chest = GetNode<Bone>("Model/ChestBone");
		var direction = chest.GlobalTransform.basis.z;
		direction = direction.Normalized() * 10;
		direction.y = 5;
		var Grenade = (PackedScene)ResourceLoader.Load("res://scenes/Grenade.tscn");;
		var grenade = (RigidBody)Grenade.Instance();
		AddChild(grenade);
		grenade.GlobalTransform = chest.GlobalTransform;
		grenade.ApplyCentralImpulse(direction);
		grenadeAmount -= 1;
		EmitSignal(nameof(GrenadeChanged), grenadeAmount);
	}
	
	void Cover(float delta)
	{
		model.SetInterpolatedPose("Cover", 1, new string[] {"RightUpperArm", "RightLowerArm", "RightHand",
				"LeftUpperArm", "LeftLowerArm", "LeftHand"});
		var idx = model.FindBone("Chest");
		model.SetBoneRest(idx, chestPose);
	}
	
	void OnRealoadTimeout()
	{
		ammo = 100;
		EmitSignal(nameof(AmmoChanged), ammo);
	}

	public override void _Ready()
	{
		camera = GetNode<Camera>("CameraTarget/Camera");
		camera.MakeCurrent();
		
		model = GetNode<Model>("Model");
		var idx = model.FindBone("Chest");
		chestPose = model.GetBoneRest(idx);
		rifleRay = GetNode<RayCast>("Model/ChestBone/RayCast");
		var reloadTimer = GetNode<Timer>("ReloadTimer");
		reloadTimer.Connect("timeout", this, nameof(OnRealoadTimeout));
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("shoot")){
			ThrowGrenade();
		}
		if (@event is InputEventMouseButton eventMouseButton) {
		} else if (@event is InputEventMouseMotion eventMouseMotion) {
			mousePosition = eventMouseMotion.Position;
			//LookAtTarget(mousePosition);
		} else if (@event.IsActionPressed("crouch"))
		{
			//SetState(States.Crouching);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		LookAtTarget(mousePosition);
		
		var baseDirection = camera.GlobalTransform.basis;
		var direction = new Vector3();
		if(Input.IsActionPressed("turn_left")) {
			RotateY(-0.1f);
		}
		if(Input.IsActionPressed("turn_right")) {
			RotateY(0.1f);
		}
		var z = baseDirection.z;
		z.y = 0;
		if(Input.IsActionPressed("move_forward")) {
			direction -= z;
		}
		if(Input.IsActionPressed("move_backward")) {
			direction += z;
		}
		if(Input.IsActionPressed("move_left")) {
			direction -= baseDirection.x;
		}
		if(Input.IsActionPressed("move_right")) {
			direction += baseDirection.x;
		}
			
		direction = direction.Normalized();
		//GD.Print(direction);
		
		velocity = velocity.LinearInterpolate(direction*speed, acceleration*delta);
		velocity += delta * gravity;
		if(Input.IsActionPressed("jump") && IsOnGround()) {
			velocity.y = 10;
		}
		
		var velocity2d = new Vector2(velocity.x, velocity.z);
		if (velocity2d.Length() > 0.1) {
			var angle = Math.Atan2((double)velocity2d.x, (double)velocity2d.y);
			var rotation = Rotation;
			rotation.y = (float)angle;
			Rotation = rotation;
		}
		
		velocity = MoveAndSlide(velocity);	
		StateContext context = new StateContext(velocity2d, 
				GlobalTransform.origin.y, 
				IsOnGround(), 
				delta);
		state.Update(context, prevContext);
		prevContext = context;
		
		if(Input.IsActionPressed("crouch")) {
		}
	
		if(Input.IsActionPressed("shoot")) {
			Shoot(delta);
			speed = 4;
		} else if (Input.IsActionPressed("cover")) {
			Cover(delta);
			speed = 2;
		} else {
			speed = 8;
		}
	}
}

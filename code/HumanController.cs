using Ducc.AI;
using Sandbox;
using Sandbox.Citizen;
using System;

public sealed partial class HumanController : Component, Component.IDamageable
{
	public delegate void DamageDelegate( DamageInfo damage );

	[Property] public DamageDelegate OnDamaged { get; set; }
	[Property] public Action<GameObject> OnKilled { get; set; }

	[Property] public CharacterController Character { get; set; }
	[Property] public SkinnedModelRenderer Renderer { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }
	[Property] public ActorComponent Actor { get; set; }
	[Property] public float MaxHealth { get; set; } = 100f;
	[Property] public Vector3 MoveDirection { get; set; }
	[Property] public Vector3? FaceDirection { get; set; }
	[Property] public GameObject FaceTarget { get; set; }

	public float CurrentHealth { get; private set; }
	public float MoveSpeed { get; set; } = 120f;

	[Property] public bool IsRunning { get; set; }
	public bool IsRagdoll { get; private set; }

	protected override void OnStart()
	{
		GameObject.BreakFromPrefab();
		CurrentHealth = MaxHealth;
	}

	protected override void OnUpdate()
	{
		UpdateAnimation();
	}

	protected override void OnFixedUpdate()
	{
		UpdateYaw();
		
		var speedFactor = IsRunning ? 2f : 1f;
		Character.Accelerate( MoveDirection * MoveSpeed * speedFactor );
		Character.ApplyFriction( 4f );
		Character.Move();
	}

	private void UpdateAnimation()
	{
		if ( !Animation.IsValid() )
			return;
		
		Animation.WithVelocity( Character.Velocity);
		Animation.WithWishVelocity( MoveDirection * MoveSpeed );
		Animation.IsGrounded = true;
	}

	private void UpdateYaw()
	{
		var faceDirection = FaceDirection ?? MoveDirection;
		if ( FaceTarget.IsValid() )
		{
			faceDirection = FaceTarget.Transform.Position - Transform.Position;
		}
		var targetRotation = Rotation.LookAt( faceDirection.Normal.WithZ( 0f ), Vector3.Up );
		var currentYaw = Renderer.GameObject.Transform.Rotation.Yaw();
		var targetYaw = MathX.Lerp( currentYaw, targetRotation.Yaw(), Time.Delta * 4f );
		Renderer.GameObject.Transform.Rotation = Rotation.FromYaw( targetYaw );
	}

	public void OnDamage( in DamageInfo damage )
	{
		CurrentHealth = MathF.Max( 0f, CurrentHealth - damage.Damage );
		IsRunning = true;
		if ( CurrentHealth <= 0f )
		{
			Kill();
		}
		OnDamaged?.Invoke( damage );
	}

	public void Kill()
	{
		SetRagdollState( true );
		Actor.Abort();
		Components.Create<Crime>();
		var decomp = Renderer.Components.Create<Decompose>();
		decomp.StartTime = 1f;
		var nearbyPeople = GetNearby( Transform.Position, 800f );
		foreach ( var person in nearbyPeople )
		{
			if ( Crime.Debug )
			{
				Log.Info( $"{person.GameObject.Name} set panicked state" );
			}
			person.IsRunning = true;
		}
		OnKilled?.Invoke( GameObject );
	}

	private void SetRagdollState( bool enabled )
	{
		var collider = Renderer.Components.Get<Collider>( FindMode.EverythingInSelf );
		if ( collider.IsValid() )
		{
			collider.Enabled = !enabled;
		}
		var physics = Renderer.Components.GetOrCreate<ModelPhysics>( FindMode.EverythingInSelf );
		physics.Enabled = enabled;
		physics.Renderer = Renderer;
		IsRagdoll = enabled;
	}
}

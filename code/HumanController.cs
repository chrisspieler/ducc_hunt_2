using Ducc.AI;
using Sandbox;
using Sandbox.Citizen;
using System;

public sealed partial class HumanController : Component, Component.IDamageable, IRagdoll
{
	public delegate void DamageDelegate( DamageInfo damage );

	[Property] public DamageDelegate OnDamaged { get; set; }
	[Property] public Action<GameObject> OnKilled { get; set; }
	[Property] public Action OnRagdollStart { get; set; }
	[Property] public Action OnRagdollEnd { get; set; }

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
			faceDirection = FaceTarget.WorldPosition - WorldPosition;
		}
		var targetRotation = Rotation.LookAt( faceDirection.Normal.WithZ( 0f ), Vector3.Up );
		var currentYaw = Renderer.GameObject.WorldRotation.Yaw();
		var targetYaw = MathX.Lerp( currentYaw, targetRotation.Yaw(), Time.Delta * 4f );
		Renderer.GameObject.WorldRotation = Rotation.FromYaw( targetYaw );
	}

	public void OnDamage( in DamageInfo damage )
	{
		if ( IsRagdoll )
			return;
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
		Renderer.SceneModel.UseAnimGraph = false;
		var crime = Components.Create<Crime>();
		crime.Victim = GameObject;
		crime.CrimeType = CrimeType.Murder;
		var decomp = Renderer.Components.Create<Decompose>();
		decomp.StartTime = 1f;
		decomp.IgnoreTime = 5f;
		var nearbyPeople = GetNearby( WorldPosition, 800f );
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

	public void SetRagdollState( bool enabled )
	{
		var collider = Renderer.Components.Get<Collider>( FindMode.EverythingInSelf );
		if ( collider.IsValid() )
		{
			collider.Enabled = !enabled;
		}
		var physics = Components.GetAll<ModelPhysics>( FindMode.EverythingInSelfAndDescendants );
		foreach( var phys in physics )
		{
			phys.Enabled = enabled;
		}
		IsRagdoll = enabled;
		var action = enabled ? OnRagdollStart : OnRagdollEnd;
		action?.Invoke();
	}
}

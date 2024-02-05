using Sandbox;
using Sandbox.Citizen;

public sealed partial class HumanController : Component, Component.IDamageable
{
	[Property] public CharacterController Character { get; set; }
	[Property] public SkinnedModelRenderer Renderer { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }

	public Vector3 MoveDirection { get; set; }
	public float MoveSpeed { get; set; } = 120f;
	public Vector3 Velocity { get; set; }

	[Property] public bool IsRunning { get; set; }
	public bool IsRagdoll { get; private set; }

	protected override void OnStart()
	{
		GameObject.BreakFromPrefab();
	}

	protected override void OnUpdate()
	{
		UpdateAnimation();
	}

	protected override void OnFixedUpdate()
	{
		Renderer.GameObject.Transform.Rotation = Rotation.LookAt( MoveDirection.Normal, Vector3.Up );

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

	public void OnDamage( in DamageInfo damage )
	{
		// Battery and murder are crimes.
		Components.Create<Crime>();
		Kill();
	}

	public void Kill()
	{
		SetRagdollState( true );
		var nearbyPeople = GetNearby( Transform.Position, 800f );
		foreach ( var person in nearbyPeople )
		{
			if ( Crime.Debug )
			{
				Log.Info( $"{person.GameObject.Name} set panicked state" );
			}
			person.IsRunning = true;
		}
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

using Sandbox;
using Sandbox.Citizen;
using System.Threading.Tasks;

public sealed partial class HumanController : Component, Component.IDamageable
{
	[Property] public CharacterController Character { get; set; }
	[Property] public SkinnedModelRenderer Renderer { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }

	public Vector3 MovementDirection { get; set; }
	public Vector3 Velocity { get; set; }
	public bool IsRagdoll { get; private set; }

	protected override void OnStart()
	{
		GameObject.BreakFromPrefab();
	}

	protected override void OnUpdate()
	{
		Think();
		
		UpdateAnimation();
	}

	protected override void OnFixedUpdate()
	{
		Renderer.GameObject.Transform.Rotation = Rotation.LookAt( Velocity.Normal, Vector3.Up );
	}

	private void UpdateAnimation()
	{
		if ( !Animation.IsValid() )
			return;
		
		Animation.WithVelocity( Velocity );
		Animation.WithWishVelocity( Velocity );
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
			Log.Info( $"{person.GameObject.Name} set panicked state" );
			person.IsPanicked = true;
		}
		Agent.Stop();
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

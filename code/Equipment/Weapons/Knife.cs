using Sandbox;
using System.Diagnostics;

public class Knife : Equipment
{
	[ConVar( "knife_debug" )]
	public static bool Debug { get; set; } = false;

	public enum KnifeState
	{
		Idle,
		Raised,
		Attacking
	}

	[Property] public SoundEvent StabSound { get; set; }
	[Property] public GameObject FleshHitParticles { get; set; }
	[Property] public GameObject StabPoint { get; set; }

	[Property] KnifeState State => _state;
	private KnifeState _state;
	

	private Vector3 _targetPosition;
	private Rotation _targetRotation;
	private Vector3 _attackPosition;

	protected override void OnUpdate()
	{
		switch ( _state )
		{
			case KnifeState.Raised:
				UpdateRaised();
				break;
			case KnifeState.Attacking:
				UpdateAttacking();
				break;
			default:
				UpdateIdle();
				break;
		}
		Transform.Position = Transform.Position.LerpTo( _targetPosition, Time.Delta * 15f );
		Transform.Rotation = Rotation.Slerp( Transform.Rotation, _targetRotation, Time.Delta * 15f );
	}

	private void BeginIdle()
	{
		_state = KnifeState.Idle;
	}

	private void UpdateIdle()
	{
		_targetPosition = Transform.Parent.Transform.World.PointToWorld( Vector3.Zero );
		_targetRotation = Rotation.Identity;
		if ( Input.Pressed( "attack1" ) )
		{
			BeginRaised();
		}
	}

	private void BeginRaised()
	{
		_state = KnifeState.Raised;
	}
	
	private void UpdateRaised()
	{
		_targetPosition = Transform.Parent.Transform.World.PointToWorld( Vector3.Up * 5f );
		_targetRotation = Transform.Parent.Transform.Rotation * Rotation.FromPitch( 90f );
		if ( !Input.Down( "attack1" ) )
		{
			BeginAttacking();
		}
	}

	private void BeginAttacking()
	{
		_state = KnifeState.Attacking;
		var attackRay = Scene.Camera.ScreenNormalToRay( new( 0.5f ) );
		_attackPosition = attackRay.Project( 160f );
	}

	private void UpdateAttacking()
	{
		if ( UpdateStabDetection() )
			return;

		_targetPosition = _attackPosition;
		var forwardRotation = Rotation.LookAt( (_attackPosition - Transform.Position).Normal );
		_targetRotation = forwardRotation * Rotation.FromPitch( 90f );
		if ( _targetPosition.Distance( Transform.Position ) <= 5f )
		{
			BeginIdle();
		}
		if ( Debug )
		{
			Gizmo.Draw.Color = Color.Blue;
			Gizmo.Draw.Line( Transform.Position, _attackPosition );
			Gizmo.Draw.LineSphere( new Sphere( _attackPosition, 2f ) );
		}
	}

	private bool UpdateStabDetection()
	{
		var stabTrace = Scene.Trace
			.Sphere( 3f, StabPoint.Transform.Position, StabPoint.Transform.Position )
			.WithoutTags( "player" )
			.Run();
		if ( stabTrace.Hit )
		{
			HandleStab( stabTrace.GameObject, stabTrace.HitPosition, stabTrace.Normal );
		}
		return stabTrace.Hit;
	}

	private void HandleStab( GameObject other, Vector3 hitPosition, Vector3 normal )
	{
		if ( Debug )
		{
			Log.Info( $"stabbed {other.Name}" );
		}
		DuccSound.Play( StabSound, other.Transform.Position );
		if ( other.Tags.Has( "fleshy" ) && FleshHitParticles.IsValid() )
		{
			var hitParticles = FleshHitParticles.Clone();
			hitParticles.Parent = other;
			hitParticles.Transform.Position = Transform.Position;
			hitParticles.Transform.Rotation = Rotation.LookAt( normal );
		}
		DoDamage( other );
		BeginIdle();
	}

	private void DoDamage( GameObject go )
	{
		if ( !go.Components.TryGet<IDamageable>( out var damageable, FindMode.EverythingInSelfAndAncestors ) )
			return;

		// TODO: Add hitbox support.
		var damageInfo = new DamageInfo( 100, DuccController.Instance.GameObject, GameObject );
		damageable.OnDamage( damageInfo );
	}
}

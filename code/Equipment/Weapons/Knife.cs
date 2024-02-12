using Sandbox;
using Sandbox.Utility;
using System;

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
	private float _attackCharge;

	protected override void OnFixedUpdate()
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
		Transform.Rotation = _targetRotation;
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
		_attackCharge = 0f;
	}
	
	private void UpdateRaised()
	{
		_attackCharge = Math.Min( _attackCharge + Time.Delta, 1f );
		_targetPosition = Transform.Parent.Transform.World.PointToWorld( Vector3.Up * 5f );
		var direction = (GetAttackPosition() - Transform.Position).Normal;
		_targetRotation = Rotation.LookAt( direction ) * Rotation.FromPitch( 90f );
		if ( !Input.Down( "attack1" ) )
		{
			BeginAttacking();
		}
	}

	private void BeginAttacking()
	{
		_state = KnifeState.Attacking;
		_attackPosition = GetAttackPosition();
	}

	private Vector3 GetAttackPosition()
	{
		var attackRay = Scene.Camera.ScreenNormalToRay( new( 0.5f ) );
		var distance = MathX.Lerp( 10f, 280f, _attackCharge );
		return attackRay.Project( distance );
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
		var ray = new Ray( StabPoint.Transform.Position, StabPoint.Transform.Rotation.Forward );
		// I can't use BBox trace because it is axis-aligned. I can't use capsules because the
		// hit postition isn't accurate. So instead I make a grid of ray traces and use the
		// first one that hits.
		var stabTrace = Scene.Trace
			.WithoutTags( "player" )
			.UseHitboxes( true )
			.RunGridTrace( ray, 17f, 2, 1f, Debug );
		if ( stabTrace.Hit )
		{
			HandleStab( stabTrace );
		}
		return stabTrace.Hit;
	}

	private void HandleStab( SceneTraceResult tr )
	{
		var other = tr.GameObject;
		if ( Debug )
		{
			Log.Info( $"stabbed {other.Name}" );
		}
		DuccSound.Play( StabSound, other.Transform.Position );
		var damage = new DamageInfo()
		{
			Attacker = DuccController.Instance.GameObject,
			Weapon = GameObject,
			Damage = 100,
			Hitbox = tr.Hitbox,
			Shape = tr.Shape,
			Position = tr.HitPosition,
			IsExplosion = false
		};
		other.DoDamage( damage );
		// Assume that anything that has a hitbox is made of flesh and blood.
		if ( tr.Hitbox is not null && FleshHitParticles.IsValid() )
		{
			var material = Material.Load( "materials/decals/blood-impact.vmat" );
			HitEffects.MakeHitEffect( tr, FleshHitParticles, material, new Vector3( 5f ) );
		}
		BeginIdle();
	}
}

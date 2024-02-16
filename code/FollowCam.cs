using Sandbox;
using System.Linq;

public sealed class FollowCam : Component
{
	[Property] public GameObject Target { get; set; }
	[Property] public float FollowDistance { get; set; }
	[Property] public float LerpSpeed { get; set; } = 20f;
	[Property] public bool InterpolateRotation { get; set; } = true;
	[Property, ShowIf("InterpolateRotation", true)] public float SlerpSpeed { get; set; } = 5f;
	[Property] bool Collide { get; set; } = false;
	[Property, ShowIf("Collide", true)] TagSet CollisionIgnoreTags { get; set; } = new();

	public Angles EyeAngles { get; set; }

	protected override void OnFixedUpdate()
	{
		if ( !Target.IsValid() )
		{
			Target = Scene.GetAllObjects( true ).FirstOrDefault( go => go.Tags.Has( "camera_target" ) );
			return;
		}

		UpdateEye();
		UpdatePosition();
		UpdateRotation();
	}

	private void UpdateEye()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch( EyeAngles.pitch.Clamp( -89, 89 ) );
	}

	private void UpdatePosition()
	{
		var orbitRay = new Ray( Target.Transform.Position, -EyeAngles.Forward );
		var targetPosition = GetTargetPosition( orbitRay );
		Transform.Position = Transform.Position.LerpTo( targetPosition, Time.Delta * LerpSpeed );
	}

	private Vector3 GetTargetPosition( Ray orbitRay )
	{
		if ( !Collide )
			return orbitRay.Project( FollowDistance );

		var tr = Scene.Trace
			.Ray( orbitRay, FollowDistance )
			.WithoutTags( CollisionIgnoreTags )
			.Run();
		var targetPosition = tr.EndPosition;
		if ( tr.Hit )
		{
			targetPosition = orbitRay.Project( tr.Distance - 1f );
		}
		return targetPosition;
	}

	private void UpdateRotation()
	{
		var direction = (Target.Transform.Position - Transform.Position).Normal;
		var targetRotation = Rotation.LookAt( direction, Vector3.Up );
		Transform.Rotation = InterpolateRotation
			? Rotation.Slerp( Transform.Rotation, targetRotation, Time.Delta * SlerpSpeed )
			: targetRotation;
	}
}

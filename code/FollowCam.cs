using Sandbox;

public sealed class FollowCam : Component
{
	[Property] public GameObject Target { get; set; }
	[Property] public float FollowDistance { get; set; }
	[Property] public float LerpSpeed { get; set; } = 20f;
	[Property] public bool InterpolateRotation { get; set; } = true;
	[Property, ShowIf("InterpolateRotation", true)] public float SlerpSpeed { get; set; } = 5f;

	public Angles EyeAngles { get; set; }

	protected override void OnUpdate()
	{
		if ( !Target.IsValid() )
			return;

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
		var targetPosition = orbitRay.Project( FollowDistance );
		Transform.Position = Transform.Position.LerpTo( targetPosition, Time.Delta * LerpSpeed );
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

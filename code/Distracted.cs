using Sandbox.Citizen;

using Sandbox;

public sealed class Distracted : Component
{
	[Property] public float Duration { get; set; } = 3f;
	[Property, Range( 0f, 1f )] public float Intensity { get; set; } = 0f;
	[Property] public GameObject Source { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }

	private TimeSince _distactionStart;

	protected override void OnStart()
	{
		Animation ??= Components.GetInAncestorsOrSelf<CitizenAnimationHelper>();
		Animation.LookAt = Source;
		_distactionStart = 0f;
		Tags.Add( "distracted" );
	}

	protected override void OnDisabled()
	{
		Animation.WithLook( Transform.Rotation.Forward, 1f, 1f, 1f );
		Tags.Remove( "distracted" );
	}

	protected override void OnUpdate()
	{
		if ( _distactionStart > Duration )
		{
			Animation.LookAt = null;
			Destroy();
			return;
		}
		var lookAt = Source.Transform.Position;
		var lookFrom = Transform.Position + Vector3.Up * 64f;
		var lookDir = (lookAt - lookFrom).Normal;
		var headWeight = MathX.Lerp( 0.2f, 1f, Intensity );
		var bodyWeight = MathX.Lerp( 0.6f, 1f, Intensity );
		Animation.WithLook( lookDir, 1f, headWeight, bodyWeight );
	}
}

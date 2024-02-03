using Sandbox.Citizen;

using Sandbox;

public sealed class Distracted : Component
{
	[Property] public float Duration { get; set; } = 3f;
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
		Animation.WithLook( lookDir, 1f, 0.2f, 0.6f );
	}
}

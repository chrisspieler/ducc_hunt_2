using Sandbox;

public sealed class Rotator : Component
{
	[Property] public Angles RotationPerSecond { get; set; }

	protected override void OnUpdate()
	{
		WorldRotation *= RotationPerSecond * Time.Delta;
	}
}

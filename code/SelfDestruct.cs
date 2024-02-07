using Sandbox;

public sealed class SelfDestruct : Component
{
	[Property] public float Delay { get; set; }

	private TimeSince _sinceStart;

	protected override void OnStart()
	{
		_sinceStart = 0f;
	}

	protected override void OnUpdate()
	{
		if ( _sinceStart >= Delay )
		{
			GameObject.Destroy();
		}
	}
}

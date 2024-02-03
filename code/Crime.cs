using Sandbox;

public sealed class Crime : Component
{
	[Property] public float SecondsSinceCrime => _crimeStart.Relative;
	private TimeSince _crimeStart;

	[Property] public bool DestroyGameObjectWithSelf { get; set; }

	protected override void OnEnabled()
	{
		_crimeStart = 0f;
	}

	protected override void OnDestroy()
	{
		if ( DestroyGameObjectWithSelf )
		{
			GameObject.Destroy();
		}
	}


}

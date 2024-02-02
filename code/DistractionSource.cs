using Sandbox;

public sealed class DistractionSource : Component
{
	[Property] public GameObject Source { get; set; }
	[Property] public float Radius { get; set; }
	[Property] public SoundEvent DistractionSound { get; set; }

	public void DeployDistraction()
	{
		Sound.Play( DistractionSound, Source.Transform.Position );
		var tr = Scene.Trace
			.Sphere( Radius, Transform.Position, Transform.Position )
			.WithAnyTags( "human" )
			.RunAll();
		foreach( var hit in tr )
		{
			var distracted = hit.GameObject.Components.GetOrCreate<Distracted>();
			distracted.Duration = 3f;
			distracted.Source = Source;
		}
	}
}

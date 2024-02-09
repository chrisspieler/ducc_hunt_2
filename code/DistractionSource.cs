using Sandbox;

public sealed class DistractionSource : Component
{
	[Property] public GameObject Source { get; set; }
	[Property] public float Radius { get; set; }
	[Property] public SoundEvent DistractionSound { get; set; }
	[Property, Range(0f, 1f)] public float DistractionIntensity { get; set; } = 0f;

	public void DeployDistraction()
	{
		DuccSound.Play( DistractionSound, Source.Transform.Position );
		var tr = Scene.Trace
			.Sphere( Radius, Transform.Position, Transform.Position )
			.WithAnyTags( "human" )
			.UseHitboxes( true )
			.RunAll();
		foreach( var hit in tr )
		{
			var distracted = hit.GameObject.Components.GetOrCreate<Distracted>();
			distracted.Duration = 3f;
			distracted.Source = Source;
			distracted.Intensity = DistractionIntensity;
		}
	}
}

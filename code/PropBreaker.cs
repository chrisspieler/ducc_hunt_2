using Sandbox;

public sealed class PropBreaker : Component
{
	[Property] public Collider Breaker { get; set; }

	protected override void OnStart()
	{
		Breaker ??= Components.Get<Collider>();
	}

	protected override void OnUpdate()
	{
		Gizmo.Draw.Color = Color.Yellow;
		var y = 0;
		foreach( var touching in Breaker.Touching )
		{
			var name = touching.GameObject.Name;
			Gizmo.Draw.ScreenText( name, new Vector2( 0, y ), "Consolas", 12, TextFlag.LeftTop );
			y += 24;
		}
	}
}

using Sandbox;
using System.Threading.Tasks;

public sealed class CrimeCam : Component
{
	[Property] public CameraComponent Camera { get; set; }
	[Property] public Crime ActiveCrime { get; set; }
	[Property] public BBox CheckArea { get; set; }

	private BBox WorldSpaceArea => CheckArea.Translate( Transform.Position );

	protected override void OnStart()
	{
		Camera ??= Components.Get<CameraComponent>();
	}

	protected override void OnUpdate()
	{
		Gizmo.Draw.Color = Color.Green;
		Gizmo.Transform = global::Transform.Zero;
		Gizmo.Draw.LineBBox( WorldSpaceArea );
		if ( ActiveCrime.IsValid() )
			return;

		if ( !TryGetNearbyCrime( out var crime ) )
			return;

		_ = ActivateCrimeCam( crime );
	}

	private bool TryGetNearbyCrime( out Crime newestCrime )
	{
		newestCrime = null;
		foreach( var crime in Scene.GetAllComponents<Crime>() )
		{
			if ( !WorldSpaceArea.Contains( crime.Transform.Position ) )
				continue;

			Log.Info( $"Crime detected on {crime.GameObject.Name}" );
			if ( newestCrime is null || crime.SecondsSinceCrime < newestCrime.SecondsSinceCrime )
			{
				newestCrime = crime;
			}
		}
		return newestCrime is not null;
	}

	private async Task ActivateCrimeCam( Crime crime)
	{
		if ( ActiveCrime.IsValid() )
			return;

		ActiveCrime = crime;
		Camera.Priority++;
		Scene.TimeScale = 0.05f;
		RealTimeUntil _unpause = 2f;
		while ( !_unpause )
		{
			await Task.Frame();
		}
		Scene.TimeScale = 1f;
		Camera.Priority--;
		ActiveCrime.Destroy();
		ActiveCrime = null;
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.White;
		Gizmo.Transform = new Transform( Transform.Position );
		Gizmo.Draw.LineBBox( CheckArea );
	}
}

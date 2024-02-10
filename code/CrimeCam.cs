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
		if ( Crime.Debug )
		{
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Transform = global::Transform.Zero;
			Gizmo.Draw.LineBBox( WorldSpaceArea );
		}
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

			if ( Crime.Debug )
			{
				Log.Info( $"Crime detected on {crime.GameObject.Name}" );
			}
			if ( newestCrime is null || crime.SecondsSinceCrime < newestCrime.SecondsSinceCrime )
			{
				newestCrime = crime;
			}
		}
		return newestCrime is not null;
	}

	private async Task ActivateCrimeCam( Crime crime, bool fullscreen = false, bool slowmo = false )
	{
		if ( ActiveCrime.IsValid() )
			return;

		ActiveCrime = crime;
		if ( fullscreen )
		{
			Camera.Priority++;
		}
		else
		{
			GameUI.SetCameraFeed( Camera ); 
		}
		if ( slowmo )
		{
			Scene.TimeScale = 0.05f;
		}
		RealTimeUntil _unpause = 2f;
		while ( !_unpause )
		{
			await Task.Frame();
		}
		if ( slowmo )
		{
			Scene.TimeScale = 1f;
		}
		if ( fullscreen )
		{
			Camera.Priority--;
		}
		else
		{
			GameUI.SetCameraFeed( null );
		}
		ActiveCrime.Destroy();
		ActiveCrime = null;
	}


	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Gizmo.IsSelected ? Color.Yellow : Color.White;
		Gizmo.Transform = new Transform( Transform.Position );
		Gizmo.Draw.LineBBox( CheckArea );
		if ( !Gizmo.IsSelected ) return;

		Gizmo.Hitbox.DepthBias = 0.02f;
		if ( Gizmo.Control.BoundingBox( "checkarea", CheckArea, out var newBox ) )
		{ 
			CheckArea = newBox;
		}
	}
}

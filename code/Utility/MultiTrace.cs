using System;

namespace Sandbox.Utility;

public static class MultiTrace
{
	public static SceneTraceResult RunGridTrace( this SceneTrace trace, Ray ray, float distance, int thickness = 1, float spacing = 1f, bool debug = false )
	{
		if ( thickness < 1 )
		{
			throw new ArgumentException( "Thickness must be at least 1, otherwise you could just be doing a single ray trace instead." );
		}

		SceneTraceResult lastTraceResult = default;

		var baseTrace = trace;
		// With the nested loops, we will run a number of traces equal to (thickness + 2)^2
		var side = 1 + thickness * 2;
		var startingOffset = -side * spacing;
		// In an x-forward system, the traces are spread along y and z.
		for( int y = 0; y < side; y++ )
		{
			for( int z = 0; z < side; z++ )
			{
				var gridOffset = new Vector3( 0, side * spacing, side * spacing ) / 2;
				var positionOffset = startingOffset + gridOffset + new Vector3( 0, y * spacing, z * spacing );
				positionOffset *= Rotation.LookAt( ray.Forward );
				var offsetRay = new Ray( ray.Position + positionOffset, ray.Forward );
				lastTraceResult = baseTrace.Ray( offsetRay, distance )
					.Run();
				if ( debug )
				{
					using ( Gizmo.Scope( "GridTraceDebug" ) )
					{
						Gizmo.Draw.Color = lastTraceResult.Hit ? Color.Yellow : Color.White;
						Gizmo.Draw.Line( offsetRay.Position, offsetRay.Project( distance ) );
					}
				}
				if ( lastTraceResult.Hit )
				{
					return lastTraceResult;
				}
			}
		}
		return lastTraceResult;
	}
}

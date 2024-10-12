using Sandbox;
using System;
using System.Diagnostics;

namespace Ducc.AI.Commands;

public class FindRandomWalkPosition : BehaviorNode
{
	public float Radius { get; set; } = float.PositiveInfinity;
	
	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		var walkTargetKey = WalkToPosition.K_WALK_POSITION;

		var sw = Stopwatch.StartNew();
		// It may take a long time to find a random point is the radius is too large.
		var radius = MathF.Min( Radius, 100_000 );
		var target = Game.ActiveScene.NavMesh.GetRandomPoint( actor.WorldPosition, radius );
		sw.Stop();
		AIDebug.Log( actor, $"New random target {target} in radius {radius} in {sw.ElapsedMilliseconds}ms" );
		if ( target.HasValue )
		{
			context.Set( walkTargetKey, target.Value );
			return BehaviorResult.Success;
		}
		else
		{
			// The NavMesh could not find the point for some reason.
			return BehaviorResult.Failure;
		}
	}
}

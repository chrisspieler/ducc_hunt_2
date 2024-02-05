using Sandbox;
using System;
using System.Diagnostics;

namespace Ducc.AI.Commands;

public class SetRandomWalkTarget : BehaviorNode
{
	public float Radius { get; set; } = float.PositiveInfinity;
	
	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		var walkTargetKey = WalkToTarget.K_WALK_TARGET;

		var sw = Stopwatch.StartNew();
		// It may take a long time to find a random point is the radius is too large.
		var radius = MathF.Min( Radius, 100_000 );
		var target = GameManager.ActiveScene.NavMesh.GetRandomPoint( actor.Transform.Position, radius );
		sw.Stop();
		if ( DebugVars.AI )
		{
			Log.Info( $"{actor.GameObject.Name}: New random target {target} in radius {radius} in {sw.ElapsedMilliseconds}ms" );
		}
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

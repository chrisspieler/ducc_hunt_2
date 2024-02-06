using Sandbox;
using System;
using System.Linq;

namespace Ducc.AI.Commands;

public class FindTaggedWalkPosition : BehaviorNode
{
	public string Tag { get; set; }
	public float Radius { get; set; } = 0f;

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		var tagged = GameManager.ActiveScene
			.GetAllObjects( true )
			.Where( go => go.Tags.Has( Tag ) )
			.ToArray();
		var target = Random.Shared.FromArray( tagged );

		if ( !target.IsValid() )
			return BehaviorResult.Failure;

		var walkTarget = target.Transform.Position;
		if ( Radius > 0f )
		{
			walkTarget = GameManager.ActiveScene
				.NavMesh
				.GetRandomPoint( walkTarget, Radius ) ?? walkTarget;
		}
		if ( DebugVars.AI )
		{
			Log.Info( $"{actor.GameObject.Name}: Navigate to {walkTarget}, distance {walkTarget.Distance( target.Transform.Position )}, radius {Radius}" );
		}

		context.Set( WalkToPosition.K_WALK_POSITION, target.Transform.Position );
		return BehaviorResult.Success;
	}


}

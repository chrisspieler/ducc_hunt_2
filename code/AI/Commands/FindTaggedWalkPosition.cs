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
		var tagged = Game.ActiveScene
			.GetAllObjects( true )
			.Where( go => go.Tags.Has( Tag ) )
			.ToArray();
		var target = Random.Shared.FromArray( tagged );

		if ( !target.IsValid() )
			return BehaviorResult.Failure;

		var walkTarget = target.WorldPosition;
		if ( Radius > 0f )
		{
			walkTarget = Game.ActiveScene
				.NavMesh
				.GetRandomPoint( walkTarget, Radius ) ?? walkTarget;
		}
		AIDebug.Log( actor, $"Navigate to {walkTarget}, distance {walkTarget.Distance( target.WorldPosition )}, radius {Radius}" );

		context.Set( WalkToPosition.K_WALK_POSITION, target.WorldPosition );
		return BehaviorResult.Success;
	}


}

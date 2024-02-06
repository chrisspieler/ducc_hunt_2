using Sandbox;
using System;
using System.Linq;

namespace Ducc.AI.Commands;

public class SetTaggedWalkTarget : BehaviorNode
{
	public string Tag { get; set; }

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		var tagged = GameManager.ActiveScene
			.GetAllObjects( true )
			.Where( go => go.Tags.Has( Tag ) )
			.ToArray();
		var target = Random.Shared.FromArray( tagged );

		if ( !target.IsValid() )
			return BehaviorResult.Failure;

		context.Set( WalkToTarget.K_WALK_TARGET, target.Transform.Position );
		return BehaviorResult.Success;
	}


}

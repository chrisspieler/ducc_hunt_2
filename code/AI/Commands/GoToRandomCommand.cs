using Sandbox;

namespace Ducc.AI;

public class GoToRandomCommand : BehaviorNode
{
	public float Radius { get; set; } = float.PositiveInfinity;
	
	public override BehaviorResult Execute( ActorComponent actor, DataContext context )
	{
		var walkTargetKey = GoToCommand.K_WALK_TARGET;

		var target = GameManager.ActiveScene.NavMesh.GetRandomPoint( actor.Transform.Position, Radius );
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

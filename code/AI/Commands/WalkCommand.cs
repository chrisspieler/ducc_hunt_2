using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Ducc.AI;

public class WalkCommand : BehaviorNode
{
	public const string K_WALK_TARGET = "walk_target";
	public const string K_PATH_GENERATION_INTERVAL = "path_generation_interval";
	public const string K_TARGET_REACHED_DISTANCE = "target_reached_distance";

	private TimeSince _sincePathGenerated;
	private List<Vector3> _pathPositions = new();
	private int _currentPathIndex = -1;

	public override BehaviorResult Execute( HumanController actor, DataContext context )
	{
		Vector3 target = context.GetVector3( K_WALK_TARGET );
		float pathGenerationInterval = context.GetFloat( K_PATH_GENERATION_INTERVAL );
		float targetReachedDistance = context.GetFloat( K_TARGET_REACHED_DISTANCE );

		var remainingDistance = actor.Transform.Position.Distance( target );
		if ( remainingDistance <= targetReachedDistance ) 
		{
			return BehaviorResult.Success;
		}

		if ( !_pathPositions.Any() || _sincePathGenerated > pathGenerationInterval )
		{
			_pathPositions = GeneratePath( actor.Transform.Position, target );
			if (!_pathPositions.Any() )
			{
				// No path to the target was found.
				return BehaviorResult.Failure;
			}
		}

		MoveActor( actor, targetReachedDistance );

		return BehaviorResult.Running;
	}

	private void MoveActor( HumanController actor, float targetReachedDistance )
	{
		var targetPos = _pathPositions[_currentPathIndex];
		if ( actor.Transform.Position.Distance( targetPos ) <= targetReachedDistance )
		{
			_currentPathIndex++;
			return;
		}
		var direction = (targetPos - actor.Transform.Position).Normal;
		actor.MovementDirection = direction;
	}

	private List<Vector3> GeneratePath( Vector3 startPos, Vector3 targetPos )
	{
		_sincePathGenerated = 0;
		var path = GameManager.ActiveScene.NavMesh.GetSimplePath( startPos, targetPos );
		_currentPathIndex = path.Any() ? 0 : -1;
		return path;
	}
}

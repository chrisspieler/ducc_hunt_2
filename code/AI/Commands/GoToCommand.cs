using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Ducc.AI;

public class GoToCommand : BehaviorNode
{
	public const string K_WALK_TARGET = "walk_target";
	public const string K_PATH_GENERATION_INTERVAL = "path_generation_interval";
	public const string K_TARGET_REACHED_DISTANCE = "target_reached_distance";

	private TimeSince _sincePathGenerated;
	private List<Vector3> _pathPositions = new();
	private int _currentPathIndex = -1;

	public override BehaviorResult Execute( ActorComponent actor, DataContext context )
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

		DebugDraw( actor );

		var human = actor.Components.Get<HumanController>();
		MoveActor( human, targetReachedDistance );

		return BehaviorResult.Running;
	}

	private void MoveActor( HumanController human, float targetReachedDistance )
	{
		var targetPos = _pathPositions[_currentPathIndex];
		if ( human.Transform.Position.Distance( targetPos ) <= targetReachedDistance )
		{
			_currentPathIndex++;
			return;
		}
		var direction = (targetPos - human.Transform.Position).Normal;
		human.MoveDirection = direction;
	}

	private List<Vector3> GeneratePath( Vector3 startPos, Vector3 targetPos )
	{
		_sincePathGenerated = 0;
		var path = GameManager.ActiveScene.NavMesh.GetSimplePath( startPos, targetPos );
		_currentPathIndex = path.Any() ? 0 : -1;
		return path;
	}

	private void DebugDraw( ActorComponent actor )
	{
		if ( !DebugVars.AI )
			return;

		using ( Gizmo.Scope( "GoToCommand", Transform.Zero ) )
		{
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Draw.LineSphere( new Sphere( actor.Transform.Position, 2f ) );
			if ( _pathPositions.Any() )
			{
				Gizmo.Draw.Line( actor.Transform.Position, _pathPositions[_currentPathIndex] );
			}
			for ( int i = 0; i < _pathPositions.Count; i++ )
			{
				Gizmo.Draw.Color = _currentPathIndex < i
					? Color.Green.WithAlpha( 0.5f )
					: Color.Green;
				Gizmo.Draw.LineSphere( new Sphere( _pathPositions[i], 0.5f ) );
				if ( i > 0 )
				{
					Gizmo.Draw.Line( _pathPositions[i - 1], _pathPositions[i] );
				}
			}
		}
		
	}
}

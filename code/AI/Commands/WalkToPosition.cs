﻿using Sandbox;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ducc.AI.Commands;

public class WalkToPosition : BehaviorNode
{
	public const string K_WALK_POSITION = "walk_position";

	public float TargetReachedDistance { get; set; } = 4.0f;

	private List<Vector3> _pathPositions = new();
	private int _currentPathIndex = -1;

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		var target = context.Get<Vector3>( K_WALK_POSITION );

		var remainingDistance = actor.WorldPosition.Distance( target );
		if ( remainingDistance <= TargetReachedDistance ) 
		{
			StopActor( actor );
			return BehaviorResult.Success;
		}

		if ( _currentPathIndex >= _pathPositions.Count )
		{
			StopActor( actor );
			return BehaviorResult.Failure;
		}

		if ( !_pathPositions.Any() )
		{
			var sw = Stopwatch.StartNew();
			_pathPositions = GeneratePath( actor.WorldPosition, target );
			sw.Stop();
			AIDebug.Log( actor, $"New {_pathPositions.Count} segment path in {sw.ElapsedMilliseconds}ms" );
			if (!_pathPositions.Any() )
			{
				// No path to the target was found.
				return BehaviorResult.Failure;
			}
		}

		AIDebug.UpdatePath( actor, _pathPositions.ToList() );

		MoveActor( actor );

		return BehaviorResult.Running;
	}

	protected override void OnAbort( ActorComponent actor, DataContext context )
	{
		StopActor( actor );
	}

	private void StopActor( ActorComponent actor )
	{
		var human = actor.Components.Get<HumanController>();
		// Face the last direction of movement.
		human.FaceDirection = human.MoveDirection;
		human.MoveDirection = Vector3.Zero;
		_pathPositions.Clear();
		_currentPathIndex = -1;
		AIDebug.UpdatePath( actor, null );
		AIDebug.Log( actor, $"Stopped walking" );
	}

	private void MoveActor( ActorComponent actor )
	{
		var human = actor.Components.Get<HumanController>();
		var targetPos = _pathPositions[_currentPathIndex];
		if ( human.WorldPosition.Distance( targetPos ) <= 4f )
		{
			_currentPathIndex++;
			AIDebug.Log( actor, $"Reached index {_currentPathIndex}" );
			return;
		}
		var direction = (targetPos - human.WorldPosition).Normal;
		human.FaceDirection = direction;
		human.MoveDirection = direction;
	}

	private List<Vector3> GeneratePath( Vector3 startPos, Vector3 targetPos )
	{
		var path = Game.ActiveScene.NavMesh.GetSimplePath( startPos, targetPos );
		_currentPathIndex = path.Any() ? 0 : -1;
		return path;
	}
}

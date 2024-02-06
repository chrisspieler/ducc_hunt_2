using Sandbox;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ducc.AI.Commands;

public class WalkToTarget : BehaviorNode
{
	public const string K_WALK_TARGET = "walk_target";

	public float TargetReachedDistance { get; set; } = 4.0f;

	private List<Vector3> _pathPositions = new();
	private int _currentPathIndex = -1;

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		Vector3 target = context.GetVector3( K_WALK_TARGET );

		var remainingDistance = actor.Transform.Position.Distance( target );
		if ( remainingDistance <= TargetReachedDistance ) 
		{
			_pathPositions.Clear();
			_currentPathIndex = -1;
			return BehaviorResult.Success;
		}

		if ( !_pathPositions.Any() )
		{
			var sw = Stopwatch.StartNew();
			_pathPositions = GeneratePath( actor.Transform.Position, target );
			sw.Stop();
			if ( DebugVars.AI )
			{
				Log.Info( $"{actor.GameObject.Name}: New {_pathPositions.Count} segment path in {sw.ElapsedMilliseconds}ms" );
			}
			if (!_pathPositions.Any() )
			{
				// No path to the target was found.
				return BehaviorResult.Failure;
			}
		}

		DebugDraw( actor );

		var human = actor.Components.Get<HumanController>();
		MoveActor( human );

		return BehaviorResult.Running;
	}

	private void MoveActor( HumanController human )
	{
		var targetPos = _pathPositions[_currentPathIndex];
		if ( human.Transform.Position.Distance( targetPos ) <= 1f )
		{
			_currentPathIndex++;
			if ( DebugVars.AI )
			{
				Log.Info( $"{human.GameObject.Name}: Reached index {_currentPathIndex}" );
			}
			return;
		}
		var direction = (targetPos - human.Transform.Position).Normal;
		human.MoveDirection = direction;
	}

	private List<Vector3> GeneratePath( Vector3 startPos, Vector3 targetPos )
	{
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

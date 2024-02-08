using Sandbox;
using Sandbox.Utility;
using System.Collections.Generic;

namespace Ducc.AI;

public static class AIDebug
{
	[ConVar( "ai_debug" )]
	public static bool ShouldDraw { get; set; } = false;

	private static Dictionary<ActorComponent, CircularBuffer<LogMessage>> _log = new();
	private static Dictionary<ActorComponent, List<Vector3>> _currentPath = new();
	private static ActorComponent _selectedActor = null;
	private static PhysicsBody _selectedBody = null;

	private struct LogMessage
	{
		public float Time { get; init; }
		public string Message { get; init; }

		public override string ToString()
		{
			return $"{Time,-10} {Message}";
		}
	}

	public static void UpdatePath( ActorComponent actor, List<Vector3> path )
	{
		_currentPath[actor] = path;
	}

	public static void Log( ActorComponent actor, string message )
	{
		if ( !_log.ContainsKey( actor ) )
		{
			_log[actor] = new CircularBuffer<LogMessage>( 20 );
		}
		_log[actor].PushBack( new LogMessage { Time = Time.Now, Message = message } );
	}

	public static void ClearLog()
	{
		foreach ( var log in _log.Values )
		{
			log.Clear();
		}
		_log.Clear();
	}

	public static void RunDebugDraw()
	{
		if ( !ShouldDraw )
			return;

		UpdateHover();
		UpdateSelectedActor();
	}

	private static void UpdateHover()
	{
		var hovered = FindHoveredActor( out var body );
		if ( Input.Pressed( "DebugAction" ) )
		{
			_selectedActor = hovered;
			_selectedBody = body;
		}

		if ( hovered is not null && hovered != _selectedActor )
		{
			Gizmo.Draw.Color = Color.White;
			Gizmo.Draw.LineBBox( body.GetBounds() );
		}
	}

	private static ActorComponent FindHoveredActor( out PhysicsBody body )
	{
		body = null;

		var scene = GameManager.ActiveScene;
		if ( !scene.IsValid() )
			return null;

		var ray = scene.Camera.ScreenNormalToRay( new( 0.5f ) );

		var tr = scene.Trace
			.Ray( ray, 2000f )
			.Radius( 10f )
			.WithTag( AISystem.ActorTag )
			.Run();
		if ( !tr.Hit || !tr.GameObject.Components.TryGet<ActorComponent>( out var actor, FindMode.EverythingInSelfAndAncestors ) )
			return null;

		body = tr.Body;
		return actor;
	}

	private static void UpdateSelectedActor()
	{
		if ( _selectedActor is null )
			return;

		if ( !_log.ContainsKey( _selectedActor ) )
			return;

		if ( _selectedBody.IsValid() )
		{
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Draw.LineBBox( _selectedBody.GetBounds() );
		}
		DrawLog( _selectedActor, _log[_selectedActor] );
		if ( _currentPath.ContainsKey( _selectedActor ) )
		{
			DrawPath( _selectedActor, _currentPath[_selectedActor] );
		}
	}

	private static ActorComponent FindActor()
	{
		var scene = GameManager.ActiveScene;
		if ( !scene.IsValid() )
			return null;

		var ray = scene.Camera.ScreenNormalToRay( new( 0.5f ) );

		var tr = scene.Trace
			.Ray( ray, 2000f )
			.Radius( 10f )
			.WithTag( "human" )
			.Run();
		if ( !tr.Hit || !tr.GameObject.Components.TryGet<ActorComponent>( out var actor, FindMode.EverythingInSelfAndAncestors ) )
			return null;

		return actor;
	}

	private static void DrawLog( ActorComponent actor, IEnumerable<LogMessage> log )
	{
		var transform = new Transform( actor.Transform.Position + Vector3.Up * 72 );
		using ( Gizmo.Scope( $"{actor.GameObject.Name} AI Log", transform ) )
		{
			Gizmo.Draw.Color = Color.Yellow;
			var x = Screen.Width / 2;
			var y = 0;
			Gizmo.Draw.ScreenText( $"\"{actor.GameObject.Name}\" AI DEBUG LOG", new Vector2( x, y ), "Consolas", 12, TextFlag.LeftTop );
			y += 18;
			Gizmo.Draw.ScreenText( "------------------------", new Vector2( x, y ), "Consolas", 12, TextFlag.LeftTop );
			y += 18;
			// y += log.Count() * 18;
			foreach ( var message in log )
			{
				Gizmo.Draw.ScreenText( message.ToString(), new Vector2( Screen.Width / 2, y ), "Consolas", 12, TextFlag.LeftTop );
				y += 18;
			}
		}
	}

	private static void DrawPath( ActorComponent actor, List<Vector3> path )
	{
		if ( path is null )
			return;

		using ( Gizmo.Scope( $"{actor.GameObject.Name} Draw Path", Transform.Zero ) )
		{
			Gizmo.Draw.Color = Color.Green;
			Gizmo.Draw.LineSphere( new Sphere( actor.Transform.Position, 2f ) );

			for ( int i = 0; i < path.Count; i++ )
			{
				Gizmo.Draw.LineSphere( new Sphere( path[i], 0.5f ) );
				if ( i > 0 )
				{
					Gizmo.Draw.Line( path[i - 1], path[i] );
				}
			}
		}
	}
}

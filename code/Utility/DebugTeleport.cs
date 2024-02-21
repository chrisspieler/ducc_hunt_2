using Sandbox;
using System.Collections.Generic;
using System.Linq;

public sealed class DebugTeleport : GameObjectSystem
{
	[ConVar( "debug_teleport" )]
	public static bool Enabled { get; set; }

	public DebugTeleport( Scene scene ) : base( scene )
	{
		Listen( Stage.UpdateBones, 0, Tick, "Debug Teleport" );
	}

	private List<GameObject> SpawnPoints = new();
	private int _currentIndex;
	private bool _hasInitialized = false;

	private void UpdateSpawnPoints()
	{
		var allSpawnPoints = Scene.GetAllComponents<SpawnPoint>();
		var devSpawns = allSpawnPoints
			.Where(IsDevSpawnPoint)
			.Select( c => c.GameObject );
		SpawnPoints.AddRange( devSpawns );
		// We might start the game at a non-dev spawn and immediately want to teleport, 
		// so make sure that non-dev spawns are last.
		var nonDevSpawns = allSpawnPoints
			.Where( c => !IsDevSpawnPoint( c ) )
			.Select( c => c.GameObject );
		SpawnPoints.AddRange( nonDevSpawns );
		_currentIndex = 0;
	}

	public static bool IsDevSpawnPoint( SpawnPoint spawn )
	{
		return spawn.Tags.Has( "debug" );
	}

	private void Tick()
	{
		if ( !_hasInitialized )
		{
			UpdateSpawnPoints();
			_hasInitialized = true;
		}
		if ( Enabled && Input.Pressed( "teleport" ) )
		{
			TeleportToNext();
		}
	}

	private void TeleportToNext()
	{
		if ( !Enabled || !SpawnPoints.Any() )
			return;

		var spawnPoint = SpawnPoints[_currentIndex];
		DuccController.Teleport( spawnPoint );
		GameUI.ShowMegaToast( spawnPoint.Name );
		_currentIndex++;
		if ( _currentIndex >= SpawnPoints.Count )
		{
			_currentIndex = 0;
		}
	}
}

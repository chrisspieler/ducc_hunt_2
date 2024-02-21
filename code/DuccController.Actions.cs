using System;
using System.Linq;

using Sandbox;

public partial class DuccController
{
	[ActionGraphNode( "ducc.get" )]
	[Title( "Get Ducc" ), Group( "Ducc" )]
	public static DuccController GetDucc()
	{
		return Instance;
	}

	[ActionGraphNode( "ducc.banish" )]
	[Title( "Banish" ), Group( "Ducc" )]
	public static void Banish()
	{
		var options = Instance.Scene.GetAllComponents<GayBabyJail>().ToArray();
		if ( !options.Any() )
		{
			Log.Info( "No gay baby jail exists on this level." );
			return;
		}
		var gbj = Random.Shared.FromArray( options );
		Instance.Transform.World = gbj.Transform.World;
		Instance.Tags.Add( "banished" );
	}

	[ActionGraphNode( "ducc.teleport" )]
	[Title( "Teleport" ), Group( "Ducc" )]
	public static void Teleport( GameObject destination )
	{
		Instance.Transform.World = destination.Transform.World.WithScale( Instance.Transform.Scale );
		Instance.Character.Velocity = 0f;
	}

	[ActionGraphNode( "ducc.respawn" )]
	[Title( "Respawn" ), Group( "Ducc" )]
	public static void Respawn()
	{
		var spawnPoint = Instance.Scene.GetAllComponents<SpawnPoint>()
			.FirstOrDefault( c => !DebugTeleport.IsDevSpawnPoint( c ) );
		if ( !spawnPoint.IsValid() )
		{
			Instance.Transform.Position = Vector3.Up * 50f;
			return;
		}
		Teleport( spawnPoint.GameObject );
	}

}

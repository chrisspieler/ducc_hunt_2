using Sandbox;

namespace Ducc.AI;

public class AISystem : GameObjectSystem
{
	[ConVar("ai_tick_rate")]
	public static int AITickRate { get; set; } = 15;

	[ConVar( "ai_actor_tag" )]
	public static string ActorTag { get; set; } = "actor";

	public AISystem( Scene scene ) : base( scene )
	{
		AIDebug.ClearLog();
		Listen( Stage.UpdateBones, 0, Tick, "AISystemTick" );
		Listen( Stage.UpdateBones, 1, AIDebug.RunDebugDraw, "AIDebugDraw" );
	}

	private RealTimeSince _sinceLastTick = 0f;

	private void Tick()
	{
		if ( _sinceLastTick < 1f / AITickRate )
			return;

		foreach( var actor in Scene.GetAllComponents<ActorComponent>() )
		{
			actor.Tick( _sinceLastTick.Relative );
		}
		_sinceLastTick = 0;
	}
}

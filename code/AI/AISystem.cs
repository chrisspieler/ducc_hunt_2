using Ducc.AI;

namespace Sandbox.AI
{
	public class AISystem : GameObjectSystem
	{
		[ConVar("ai_tick_rate")]
		public static int AITickRate { get; set; } = 15;
		public AISystem( Scene scene ) : base( scene )
		{
			Listen( Stage.UpdateBones, 0, Tick, "AISystemTick" );
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
}

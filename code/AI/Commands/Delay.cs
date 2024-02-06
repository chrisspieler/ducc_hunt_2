using Sandbox;

namespace Ducc.AI.Commands;

public class Delay : BehaviorNode
{
	public float Duration { get; set; } = 1f;

	private TimeSince _sinceStartDelay;

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		if ( _sinceStartDelay >= Duration )
		{
			return BehaviorResult.Success;
		}
		return BehaviorResult.Running;
	}

	protected override void OnStart( ActorComponent actor, DataContext context )
	{
		if ( DebugVars.AI )
		{
			Log.Info( $"{actor.GameObject.Name}: Delay for {Duration}s" );
		}
		_sinceStartDelay = 0f;
	}
}

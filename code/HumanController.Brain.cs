using Sandbox;

public partial class HumanController
{
	[ConVar("ai_debug")]
	public static bool Debug { get; set; } = false;

	[Property] public NavMeshAgent Agent { get; set; }
	[Property] public bool IsPanicked { get; set; }
	
	private void Think()
	{
		if ( IsRagdoll )
			return;

		Agent.MaxSpeed = IsPanicked ? 300f : 80f;
		if ( !IsNavigating() )
		{
			var random = Scene.NavMesh.GetRandomPoint( Transform.Position, 1000f );
			if ( random.HasValue )
			{
				Agent.MoveTo( random.Value );
			}
		}
		MovementDirection = Agent.WishVelocity.Normal;
		Velocity = Agent.Velocity;
		DebugDraw();
	}

	private void DebugDraw()
	{
		if ( !Debug )
			return;

		Gizmo.Draw.Color = Color.Green;
		Gizmo.Draw.LineSphere( new Sphere( Agent.AgentPosition, 2f ) );
		if ( Agent.TargetPosition.HasValue )
		{
			Gizmo.Draw.Line( Agent.AgentPosition, Agent.TargetPosition.Value );
		}
	}

	private bool IsNavigating()
	{
		return Agent.TargetPosition.HasValue
			&& Agent.TargetPosition.Value.Distance( Transform.Position ) > 5f;
	}
}

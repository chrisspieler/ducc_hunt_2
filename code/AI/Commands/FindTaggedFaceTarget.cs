using Sandbox;
using System;
using System.Linq;

namespace Ducc.AI.Commands;

public class FindTaggedFaceTarget : BehaviorNode
{
	public string Tag { get; set; }

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		var tagged = GameManager.ActiveScene
			.GetAllObjects( true )
			.Where( go => go.Tags.Has( Tag ) )
			.ToArray();
		var target = Random.Shared.FromArray( tagged );

		if ( !target.IsValid() )
		{
			AIDebug.Log( actor, $"No valid target found with tag {Tag}" );
			return BehaviorResult.Failure;
		}

		context.Set( SetFaceTarget.K_FACE_TARGET, target.Id );
		AIDebug.Log( actor, $"Found face target {target.Name} tagged \"{Tag}\"" );
		return BehaviorResult.Success;
	}
}

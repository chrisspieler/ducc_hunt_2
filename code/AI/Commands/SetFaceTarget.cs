using Sandbox;
using System;

namespace Ducc.AI.Commands
{
	public class SetFaceTarget : BehaviorNode
	{
		public bool SetNull { get; set; } = false;

		public const string K_FACE_TARGET = "face_target";
		
		protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
		{
			GameObject targetGo = null;
			if ( !SetNull )
			{
				var target = context.Get<Guid>( K_FACE_TARGET );
				targetGo = actor.Scene.Directory.FindByGuid( target );
				if ( targetGo is null )
				{
					AIDebug.Log( actor, $"No valid target found with guid {target}" );
					return BehaviorResult.Failure;
				}
			}
			var human = actor.Components.Get<HumanController>();
			AIDebug.Log( actor, $"Setting face target to {targetGo?.Name ?? "null"}" );
			human.FaceTarget = targetGo;
			return BehaviorResult.Success;
		}
	}
}

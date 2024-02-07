using System.Collections.Generic;
using System.Linq;

namespace Ducc.AI;

public class Selector : BehaviorNode
{
	private readonly HashSet<BehaviorNode> _failedSubtasks = new();
	
	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		foreach( var subtask in Subtasks )
		{
			if ( _failedSubtasks.Contains( subtask ) )
				continue;

			var result = subtask.Execute( actor, context );
			switch ( result )
			{
				case BehaviorResult.Success:
					_failedSubtasks.Clear();
					return BehaviorResult.Success;
				case BehaviorResult.Running:
					return BehaviorResult.Running;
				case BehaviorResult.Failure:
					_failedSubtasks.Add( subtask );
					break;
			}
		}
		// Either all subtasks have failed, or there are no subtasks.
		return Subtasks.Any() ? BehaviorResult.Failure : BehaviorResult.Success;
	}

	protected override void OnAbort( ActorComponent actor, DataContext context )
	{
		_failedSubtasks.Clear();
	}
}

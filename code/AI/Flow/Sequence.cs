using System.Collections.Generic;

namespace Ducc.AI;

public class Sequence : BehaviorNode
{
	private HashSet<BehaviorNode> _successfulSubtasks = new();

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		foreach( var subtask in Subtasks )
		{
			// We already succeeded at this subtask.
			if ( _successfulSubtasks.Contains( subtask ) )
				continue;

			var result = subtask.Execute( actor, context );
			switch ( result )
			{
				case BehaviorResult.Success:
					_successfulSubtasks.Add( subtask );
					break;
				case BehaviorResult.Running:
					return BehaviorResult.Running;
				case BehaviorResult.Failure:
					_successfulSubtasks.Clear();
					return BehaviorResult.Failure;
			}
		}
		// There weren't any subtasks, or we succeeded at all of them.
		_successfulSubtasks.Clear();
		return BehaviorResult.Success;
	}
}

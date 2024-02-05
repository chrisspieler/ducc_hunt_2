namespace Ducc.AI.Commands;

public class ExecuteBehaviorTree : BehaviorNode
{
	public string BehaviorTreeName { get; set; }

	private BehaviorTree _behaviorTree;

	protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
	{
		_behaviorTree ??= BehaviorTree.Load( BehaviorTreeName );
		return _behaviorTree.Execute( actor, context );
	}
}

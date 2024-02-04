namespace Ducc.AI;

public class ExecuteBehaviorTreeCommand : BehaviorNode
{
	public string BehaviorTreeName { get; set; }

	private BehaviorTree _behaviorTree;

	public override BehaviorResult Execute( ActorComponent actor, DataContext context )
	{
		_behaviorTree ??= BehaviorTree.Load( BehaviorTreeName );
		return _behaviorTree.Execute( actor, context );
	}
}

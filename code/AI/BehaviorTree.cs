using Sandbox;

namespace Ducc.AI;

public class BehaviorTree : BehaviorNode
{
	public string Name { get; set; } = "Behavior Tree";
	public BehaviorNode Root { get; set; }

	public override BehaviorResult Execute( HumanController actor, DataContext context )
	{
		// If this behavior tree is acting as a leaf node in another tree,
		// its context should be provided by the containing tree.
		var treeContext = context ?? new DataContext();

		return Root.Execute( actor, treeContext );
	}
}

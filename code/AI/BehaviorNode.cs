namespace Ducc.AI;

public abstract class BehaviorNode
{
	public virtual void OnStart( HumanController actor, DataContext context ) { }
	public abstract BehaviorResult Execute( HumanController actor, DataContext context );
	public virtual void OnStop( HumanController actor, DataContext context ) { }
}

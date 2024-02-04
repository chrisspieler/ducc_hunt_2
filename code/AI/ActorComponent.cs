using Sandbox;

namespace Ducc.AI;

public class ActorComponent : Component
{
	[Property] public string InitialTree { get; set; }
	public BehaviorTree CurrentTree 
	{
		get => _currentTree;
		set
		{
			_currentTree = value;
		}
	}
	private BehaviorTree _currentTree;
	public DataContext DataContext 
	{
		get => _dataContext ?? new DataContext();
		set => _dataContext = value;
	}
	private DataContext _dataContext;

	protected override void OnUpdate()
	{
		CurrentTree?.Execute( this, DataContext );
	}
}

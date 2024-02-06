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
			if ( _currentTree is not null )
			{
				Abort();
			}
			_currentTree = value;
		}
	}
	private BehaviorTree _currentTree;
	public DataContext DataContext 
	{
		get
		{
			_dataContext ??= new();
			return _dataContext;
		}
		set => _dataContext = value;
	}
	private DataContext _dataContext;

	protected override void OnStart()
	{
		SetTree( InitialTree );
	}

	protected override void OnUpdate()
	{
		CurrentTree?.Execute( this, DataContext );
	}

	public void SetTree( string treeName )
	{
		CurrentTree = BehaviorTree.Load( treeName );
	}

	public void Abort()
	{
		_currentTree?.Abort();
		_currentTree = null;
	}
}

using Sandbox;

namespace Ducc.AI;

public class ActorComponent : Component
{
	public const string K_DELTA_TIME = "delta_time";

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

	public void Tick( float deltaTime )
	{
		DataContext.Set( K_DELTA_TIME, deltaTime );
		CurrentTree?.Execute( this, DataContext );
	}

	public void SetTree( string treeName )
	{
		if ( string.IsNullOrWhiteSpace( treeName ) )
		{
			CurrentTree = null;
		}
		CurrentTree = BehaviorTree.Load( treeName );
		if ( CurrentTree is null )
		{
			Log.Error( "Unable to find behavior tree: " + treeName );
		}
	}

	public void Abort()
	{
		_currentTree?.Abort( this, DataContext );
		_currentTree = null;
	}
}

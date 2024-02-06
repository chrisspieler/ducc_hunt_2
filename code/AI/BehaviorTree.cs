using Ducc.AI.Commands;
using Sandbox;
using Sandbox.Diagnostics;
using System.Text.Json.Nodes;

namespace Ducc.AI;

public class BehaviorTree
{
	public BehaviorNode Root { get; set; }

	public BehaviorResult Execute( ActorComponent actor, DataContext context )
	{
		Assert.NotNull( context );
		Assert.NotNull( actor );
		return Root.Execute( actor, context );
	}

	public void Abort()
	{
		Root?.Abort();
	}

	public JsonNode Serialize()
	{
		var node = Json.ToNode( new BehaviorTree() );
		node[nameof( Root )] = Root.Serialize();
		return node;
	}

	public static BehaviorTree Deserialize( JsonObject json )
	{
		return new BehaviorTree
		{
			Root = BehaviorNode.Deserialize( json[nameof( Root )] )
		};
	}

	/// <summary>
	/// Returns a BehaviorTree with the given name, or null if it does not exist.
	/// </summary>
	public static BehaviorTree Load( string name )
	{
		var filePath = $"data/behavior/{name}.json";
		if ( !FileSystem.Mounted.FileExists( filePath ) )
		{
			return null;
		}
		var json = FileSystem.Mounted.ReadJson<JsonObject>( filePath );
		return Deserialize( json );
	}

	[ConCmd("ai_test_load")]
	public static void TestLoad()
	{
		var tree = Load( "TestBehaviorTree" );
		Log.Info( tree.Serialize().ToString() );
	}

	[ConCmd("ai_test_save")]
	public static void TestSave()
	{
		var tree = new BehaviorTree();
		tree.Root = new Sequence()
		{
			Subtasks = new()
			{
				new SetRandomWalkTarget() { Radius = 30f },
				new WalkToTarget()
			}
		};
		var node = tree.Serialize();
		FileSystem.Data.WriteAllText( $"TestBehaviorTree.json", node.ToString() );
	}
}

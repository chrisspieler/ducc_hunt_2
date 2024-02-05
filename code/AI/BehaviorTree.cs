using Ducc.AI.Commands;
using Sandbox;
using System.Text.Json.Nodes;

namespace Ducc.AI;

public class BehaviorTree
{
	public BehaviorNode Root { get; set; }

	public BehaviorResult Execute( ActorComponent actor, DataContext context )
	{
		// If this behavior tree is acting as a leaf node in another tree,
		// its context should be provided by the containing tree.
		var treeContext = context ?? new DataContext();

		return Root.Execute( actor, treeContext );
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

	public static BehaviorTree Load( string name )
	{
		var filePath = $"data/behavior/{name}.json";
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

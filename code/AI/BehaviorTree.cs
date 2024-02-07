using Ducc.AI.Commands;
using Ducc.AI.Flow;
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

	public void Abort( ActorComponent actor, DataContext context )
	{
		Root?.Abort( actor, context );
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
	public static void TestLoad( string treeName )
	{
		var tree = Load( treeName );
		if ( tree is null )
		{
			Log.Info( "Unable to find behavior tree: " + treeName );
		}
		Log.Info( tree.Serialize().ToString() );
	}

	[ConCmd("ai_save_all")]
	public static void SaveAll()
	{
		Save( "Wander", BuildWanderTree() );
		Save( "FollowPlayer", BuildFollowTree() );
	}

	private static void Save( string name, BehaviorTree tree )
	{
		var node = tree.Serialize();
		FileSystem.Data.WriteAllText( $"{name}.json", node.ToString() );
	}

	private static BehaviorTree BuildWanderTree()
	{
		return new BehaviorTree()
		{
			Root = new Sequence()
			{
				Subtasks = new()
				{
					new FindRandomWalkPosition() { Radius = 100f },
					new WalkToPosition(),
					new Delay() { Duration = 3f }
				}
			}
		};
	}

	private static BehaviorTree BuildFollowTree()
	{
		return new BehaviorTree()
		{
			Root = new Sequence()
			{
				Subtasks = new()
				{
					new HasKeyConditional() { Key = "player_tag_target" },
					new FindTaggedWalkPosition() { Tag = "player", Radius = 80f },
					new WalkToPosition() { TargetReachedDistance = 80f },
					new FindTaggedFaceTarget() { Tag = "player" },
					new SetFaceTarget(),
					new Delay() { Duration = 2f },
					new SetFaceTarget() { SetNull = true }
				}
			}
		};
	}
}

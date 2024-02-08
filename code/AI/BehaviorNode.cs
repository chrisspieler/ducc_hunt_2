using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Ducc.AI;

public abstract partial class BehaviorNode
{
	[JsonPropertyName( "_type" )]
	public string TypeName { get; }
	public List<BehaviorNode> Subtasks { get; init; } = new();
	public BehaviorResult LastResult { get; private set; }

	public BehaviorNode()
	{
		TypeName = GetType().Name;
	}

	public BehaviorResult Execute( ActorComponent actor, DataContext context )
	{
		// If we weren't running last time, then we're starting fresh.
		if ( LastResult != BehaviorResult.Running )
		{
			OnStart( actor, context );
		}
		LastResult = ExecuteInternal( actor, context );
		return LastResult;
	}

	protected abstract BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context );

	protected virtual void OnStart( ActorComponent actor, DataContext context ) { }

	public void Abort( ActorComponent actor, DataContext context )
	{
		foreach( var subtask in Subtasks )
		{
			subtask.Abort( actor, context );
		}
		OnAbort( actor, context );
	}

	protected virtual void OnAbort( ActorComponent actor, DataContext context ) { }

	public JsonNode Serialize()
	{
		var type = TypeLibrary.GetType( GetType() );
		var json = JsonSerializer.SerializeToNode( this, type.TargetType );
		json["_type"] = TypeName;
		var jsonSubtasks = new JsonArray();
		json["Subtasks"] = jsonSubtasks;
		foreach ( var subtask in Subtasks )
		{
			jsonSubtasks.Add( subtask.Serialize() );
		}
		return json;
	}

	public static BehaviorNode Deserialize( JsonNode jsonNode )
	{
		var typeName = jsonNode["_type"].ToString();
		var typeInfo = TypeLibrary.GetType( typeName );
		// For some reason, I can't deserialize subclasses of BehaviorNode,
		// so here I'm creating a new instance that I'll populate with 
		// information from TypeLibrary.
		var node = TypeLibrary.Create<BehaviorNode>( typeName );
		foreach ( var property in typeInfo.Properties )
		{
			// Skip these properties because they are handled elsewhere.
			if ( property.Name == "Subtasks" || property.Name == "TypeName" )
				continue;

			var jsonValue = jsonNode[property.Name];
			var stringValue = jsonValue?.ToString();
			property.SetValue( node, stringValue );
		}
		var subtasks = jsonNode["Subtasks"].AsArray();
		foreach ( var subtask in subtasks )
		{
			node.Subtasks.Add( Deserialize( subtask ) );
		}
		return node;
	}
}

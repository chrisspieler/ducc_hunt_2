using System.Collections.Generic;
using System.Linq;

using Sandbox;

public partial class HumanController
{
	public static IEnumerable<HumanController> GetNearby( Vector3 position, float radius )
	{
		return Game.ActiveScene.Trace
			.Sphere( radius, position, position )
			.WithAllTags( "human" )
			.RunAll()
			.Select( c => c.GameObject.Components.GetInAncestorsOrSelf<HumanController>() )
			.Where( c => c.IsValid() );
	}
}

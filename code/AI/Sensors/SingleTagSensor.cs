using Sandbox;
using System;
using System.Linq;

namespace Ducc.AI.Sensors
{
	public sealed class SingleTagSensor : Component
	{
		[Property] public GameObject SensorOrigin { get; set; }
		[Property] public string Tag { get; set; } = "player";
		[Property] public float Radius { get; set; } = 800f;
		[Property] public ActorComponent Actor { get; set; } 

		public static string GetTagTargetKey( string tag ) => $"{tag}_tag_target";
		public string TagTargetKey => GetTagTargetKey( Tag );

		protected override void OnStart()
		{
			SensorOrigin ??= GameObject;
			Actor ??= Components.Get<ActorComponent>();
		}

		protected override void OnUpdate()
		{
			GameObject target = Radius > 0f ? FindByRadius( Radius ) : FindByTag();
			if ( target is null )
			{
				Actor.DataContext.Remove<Guid>( TagTargetKey );
			}
			else
			{
				Actor.DataContext.Set( TagTargetKey, target.Id );
			}
		}

		private GameObject FindByTag()
		{
			return Scene.GetAllObjects( true )
				.FirstOrDefault( go => go.Tags.Has( Tag ) );
		}

		private GameObject FindByRadius( float radius )
		{
			var startPos = SensorOrigin.WorldPosition;
			var tr = Scene.Trace
				.Sphere( Radius, startPos, startPos )
				.WithAllTags( Tag )
				.Run();
			return tr.GameObject;
		}
	}
}

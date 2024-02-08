using System.Linq;

namespace Sandbox;

public sealed class Decompose : Component
{
	[Property] public ModelRenderer Renderer { get; set; }
	[Property] public Model DecomposedModel { get; set; }
	[Property] public float StartTime { get; set; } = 20f;
	[Property] public bool OnlyWhenLookingAway { get; set; } = true;

	private TimeSince _sinceStart;

	protected override void OnStart()
	{
		_sinceStart = 0f;

		Renderer ??= Components.Get<SkinnedModelRenderer>();
		DecomposedModel ??= Model.Load( "models/citizen/skin/skeleton/models/skeleton_skin.vmdl" );
	}

	protected override void OnUpdate()
	{
		if ( _sinceStart > StartTime && CanDecompose() )
		{
			RotClothing();
			Skeletonize();
			Destroy();
		}
	}

	public bool CanDecompose()
	{
		if ( !OnlyWhenLookingAway )
			return true;

		var camDirection = Scene.Camera.Transform.Rotation.Forward;
		var camToCorpse = (Renderer.Transform.Position - Scene.Camera.Transform.Position).Normal;
		// Only decompose if the camera is looking away from the corpse.
		return camToCorpse.Dot( camDirection ) < 0f;
	}

	private void RotClothing()
	{
		var toDestroy = Components
			.GetAll<SkinnedModelRenderer>( FindMode.InDescendants )
			.Where( c => c.Tags.Has( "clothing" ) )
			.ToList();
		foreach( var clothing in toDestroy )
		{
			clothing.GameObject.Destroy();
		}
	}

	private void Skeletonize()
	{
		Renderer.Model = DecomposedModel;
	}
}

using System;

namespace Sandbox;

public static class RagdollDecal
{
	public static DecalRenderer FromTrace( SceneTraceResult tr, SkinnedModelRenderer model, Material decalMat, Vector3 size )
	{
		var bone = model.GetBoneObject( tr.Bone );
		var gameObject = new GameObject( true, "Ragdoll Decal" );
		gameObject.Parent = bone;
		// Project the decal on to the surface, but back it up a little so that
		// the decal doesn't clip in to the surface or clothing.
		gameObject.Transform.Position = tr.HitPosition + tr.Normal * 1f;
		gameObject.Transform.Rotation = Rotation.LookAt( -tr.Normal );
		// Randomize the rotation.
		gameObject.Transform.Rotation *= Rotation.FromRoll( Random.Shared.Float( 0, 360 ) );
		var decal = gameObject.Components.Create<DecalRenderer>();
		decal.Material = decalMat;
		decal.Size = size;
		return decal;
	}
}

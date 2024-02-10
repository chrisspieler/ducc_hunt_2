using System;

namespace Sandbox.Utility
{
	public static class HitEffects
	{
		public static void MakeHitEffect( SceneTraceResult tr, GameObject hitPrefab, Material decalMat, Vector3 size )
		{
			var parent = tr.GameObject;
			var find = FindMode.EverythingInSelfAndAncestors;
			if ( tr.GameObject.Components.TryGet<SkinnedModelRenderer>( out var renderer, find ) )
			{
				parent = renderer.GetBoneObject( tr.Bone );
			}

			MakeHitEffect( tr, hitPrefab, parent );
			MakeDecal( tr, parent, decalMat, size );
		}

		public static GameObject MakeHitEffect( SceneTraceResult tr, GameObject hitPrefab, GameObject parent )
		{
			var hitEffect = hitPrefab.Clone();
			hitEffect.Parent = parent;
			hitEffect.Transform.Position = tr.HitPosition + tr.Normal;
			hitEffect.Transform.Rotation = Rotation.LookAt( tr.Normal );
			return hitEffect;
		}

		public static DecalRenderer MakeDecal( SceneTraceResult tr, GameObject parent, Material decalMat, Vector3 size )
		{
			var gameObject = new GameObject( true, "Ragdoll Decal" );
			gameObject.Parent = parent;
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
}

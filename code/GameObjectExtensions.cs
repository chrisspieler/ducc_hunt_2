namespace Sandbox
{
	public static class GameObjectExtensions
	{
		public static void DoDamage( this GameObject go, DamageInfo damage )
		{
			// Usually the component that controls a player/prop will also handle
			// damage and be a sibling or ancestor of the collider.
			var find = FindMode.EverythingInSelfAndAncestors;
			if ( !go.Components.TryGet<Component.IDamageable>( out var damageable, find ) )
				return;
			damageable.OnDamage( damage );
		}
	}
}

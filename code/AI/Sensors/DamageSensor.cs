using Ducc.AI;

namespace Sandbox.AI.Sensors
{
	public sealed class DamageSensor : Component
	{
		public const string K_LAST_DAMAGE_SOURCE = "last_damage_source";
		public const string K_LAST_DAMAGE_TIME = "last_damage_time";
		public const string K_LAST_DAMAGE_AMOUNT = "last_damage_amount";
		public const string K_HEALTH_PERCENT = "health_percent";

		[Property] public HumanController Controller { get; set; }
		[Property] public ActorComponent Actor { get; set; }

		protected override void OnStart()
		{
			Controller ??= Components.Get<HumanController>();
			Controller.OnDamaged += OnDamage;
			Actor ??= Components.Get<ActorComponent>();
		}

		protected override void OnUpdate()
		{
			Actor.DataContext.Set( K_HEALTH_PERCENT, Controller.CurrentHealth / Controller.MaxHealth );
		}

		private void OnDamage( DamageInfo info )
		{
			Actor.DataContext.Set( K_LAST_DAMAGE_SOURCE, info.Attacker.Id );
			Actor.DataContext.Set( K_LAST_DAMAGE_TIME, Time.Now );
			Actor.DataContext.Set( K_LAST_DAMAGE_AMOUNT, info.Damage );
		}
	}
}

using Sandbox;

public sealed class BossEnemy : Component
{
	[Property] public string BossName { get; set; } = "Some Random Guy";
	[Property] public HumanController Human { get; set; }
	[Property] public bool IsFighting => _isFighting;
	private bool _isFighting;
	[Property] public MusicComponent BossMusic { get; set; }

	protected override void OnStart()
	{
		Human ??= Components.Get<HumanController>();
		Human.OnDamaged += HandleOnDamage;
	}

	public void HandleOnDamage( DamageInfo damage )
	{
		if ( IsFighting )
		{
			if ( Human.CurrentHealth <= 0f )
			{
				EndFight();
				return;
			}
			GameUI.ShowBossPanel( BossName, Human.CurrentHealth / Human.MaxHealth );
			return;
		}

		StartFight();
	}

	public void StartFight()
	{
		if ( !Human.IsValid() || Human.CurrentHealth <= 0f )
		{
			return;
		}

		_isFighting = true;

		if ( BossMusic.IsValid() )
		{
			BossMusic.Play();
		}

		GameUI.ShowBossPanel( BossName, Human.CurrentHealth / Human.MaxHealth );
	}

	public void EndFight()
	{
		_isFighting = false;
		if ( BossMusic.IsValid() )
		{
			BossMusic.Stop();
		}
		GameUI.CloseBossPanel();
	}
}

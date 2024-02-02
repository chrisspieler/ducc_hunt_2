using Sandbox;
using Sandbox.Citizen;
using System.Threading.Tasks;

public sealed class HumanController : Component, Component.IDamageable
{
	[Property] public CharacterController Character { get; set; }
	[Property] public SkinnedModelRenderer Renderer { get; set; }
	[Property] public CitizenAnimationHelper Animation { get; set; }

	public bool IsRagdoll { get; private set; }

	protected override void OnUpdate()
	{

	}

	private void UpdateAnimation()
	{
		
	}

	public void OnDamage( in DamageInfo damage )
	{
		SetRagdollState( !IsRagdoll );
		_ = MurderCam();
	}

	private async Task MurderCam()
	{
		var mainCam = Scene.Camera;
		mainCam.Priority--;
		Scene.TimeScale = 0.05f;
		RealTimeUntil _unpause = 2f;
		while( !_unpause )
		{
			await Task.Frame();
		}
		Scene.TimeScale = 1f;
		mainCam.Priority++;
	}

	private void SetRagdollState( bool enabled )
	{
		// var collider = Renderer.Components.GetOrCreate<ModelCollider>( FindMode.EverythingInSelf );
		// collider.Enabled = !enabled;
		var physics = Renderer.Components.GetOrCreate<ModelPhysics>( FindMode.EverythingInSelf );
		physics.Enabled = enabled;
		physics.Renderer = Renderer;
		IsRagdoll = enabled;
	}
}

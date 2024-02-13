using System.Collections.Generic;

namespace Sandbox;

public sealed class RagdollMerge : Component
{
	[Property] public SkinnedModelRenderer RagdollModel { get; set; }
	[Property] public ModelPhysics RagdollPhysics { get; set; }
	[Property] public List<SkinnedModelRenderer> PuppetModels { get; set; }
	[Property] public Component Controller 
	{
		get => _controller;
		set
		{
			if ( value is IRagdoll )
			{
				_controller = value;
				return;
			}
			_controller = value.GameObject.Components.Get<IRagdoll>() as Component;
		}
	}
	private Component _controller;

	protected override void OnStart()
	{
		var ragdoller = Controller as IRagdoll;
		ragdoller.OnRagdollStart += OnRagdollStart;
		ragdoller.OnRagdollEnd += OnRagdollEnd;
	}

	private void OnRagdollStart()
	{
		foreach( var model in PuppetModels )
		{
			model.BoneMergeTarget = RagdollModel;
		}
		RagdollPhysics.Enabled = true;
	}

	private void OnRagdollEnd()
	{
		foreach( var model in PuppetModels )
		{
			model.BoneMergeTarget = null;
		}
		RagdollPhysics.Enabled = false;
	}
}

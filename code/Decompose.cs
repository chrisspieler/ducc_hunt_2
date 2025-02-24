﻿using System;
using System.Linq;

namespace Sandbox;

public sealed class Decompose : Component
{
	[Property] public Action OnDecomposed { get; set; }

	[Property] public ModelRenderer Renderer { get; set; }
	[Property] public Model DecomposedModel { get; set; }
	[Property] public float StartTime { get; set; } = 20f;
	[Property] public float IgnoreTime { get; set; } = 5f;
	[Property] public bool OnlyWhenLookingAway { get; set; } = true;

	private TimeSince _sinceStart;
	private TimeSince _sinceIgnoreStart;

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
			RemoveDecals();
			DisableHitboxes();
			OnDecomposed?.Invoke();
			Destroy();
		}
	}

	public bool CanDecompose()
	{
		if ( !OnlyWhenLookingAway )
			return true;

		var isLookingAway = IsLookingAway();
		if ( !isLookingAway )
		{
			_sinceIgnoreStart = 0f;
		}
		return isLookingAway && _sinceIgnoreStart > IgnoreTime;
	}

	private bool IsLookingAway()
	{
		var camDirection = Scene.Camera.WorldRotation.Forward;
		var camToCorpse = (Renderer.WorldPosition - Scene.Camera.WorldPosition).Normal;
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

	private void RemoveDecals()
	{
		var toDestroy = Components
			.GetAll<DecalRenderer>( FindMode.InDescendants )
			.ToList();
		foreach ( var decal in toDestroy )
		{
			decal.GameObject.Destroy();
		}
	}

	private void DisableHitboxes()
	{
		var toDisable = Components.Get<ModelHitboxes>( FindMode.EnabledInSelfAndDescendants );
		if ( toDisable != null )
		{
			toDisable.Enabled = false;
		}
	}

	private void Skeletonize()
	{
		Renderer.Model = DecomposedModel;
	}
}

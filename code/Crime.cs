﻿using Sandbox;

public sealed class Crime : Component
{
	[ConVar( "crime_debug" )]
	public static bool Debug { get; set; }

	[Property] public GameObject Victim { get; set; }
	[Property] public CrimeType CrimeType { get; set; }
	[Property] public float SecondsSinceCrime => _crimeStart.Relative;
	private TimeSince _crimeStart;

	[Property] public bool DestroyGameObjectWithSelf { get; set; }

	protected override void OnEnabled()
	{
		_crimeStart = 0f;
	}

	protected override void OnDestroy()
	{
		if ( DestroyGameObjectWithSelf )
		{
			GameObject.Destroy();
		}
	}
}

public enum CrimeType
{
	Murder
}

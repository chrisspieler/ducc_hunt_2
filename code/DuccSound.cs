using System;
using System.Collections.Generic;
using System.Linq;

namespace Sandbox
{
	public class DuccSound : GameObjectSystem
	{
		[ConCmd("duccsound_status")]
		public static void PrintSoundStatus()
		{
			Log.Info( "Active sounds: " + Current._activeSounds.Count );
		}

		public DuccSound( Scene scene ) : base( scene )
		{
			Listen( Stage.UpdateBones, 0, Tick, "TickDuccSound" );
		}

		[ConVar( "volume_sound" )]
		public static float GlobalSoundVolume { get; set; } = 1f;
		[ConVar( "volume_music" )]
		public static float GlobalMusicVolume { get; set; } = 1.0f;

		private Dictionary<DuccSoundHandle, SoundHandle> _activeSounds = new();

		public static DuccSound Current => GameManager.ActiveScene.GetSystem<DuccSound>();

		public static DuccSoundHandle Play( SoundEvent sound, GameObject follow = null )
		{
			var hSnd = Sound.Play( sound );
			var duccSnd = new DuccSoundHandle( hSnd )
			{
				Follow = follow
			};
			hSnd.Volume = GetEffectiveVolume( duccSnd );
			hSnd.Pitch = GetEffectivePitch( duccSnd );
			Current._activeSounds.Add( duccSnd, hSnd );
			return new DuccSoundHandle( hSnd );
		}

		public static DuccSoundHandle Play( SoundEvent sound, Vector3 position )
		{
			var duccSnd = Play( sound );
			duccSnd.Position = position;
			return duccSnd;
		}

		public void Tick()
		{
			var allSounds = Current._activeSounds.ToArray();
			foreach( var (duccSnd, hSnd) in allSounds )
			{
				if ( !hSnd.IsPlaying )
				{
					Current._activeSounds.Remove( duccSnd );
					continue;
				}
				hSnd.Volume = GetEffectiveVolume( duccSnd );
				hSnd.Pitch = GetEffectivePitch( duccSnd );
				if ( duccSnd.Follow.IsValid() )
				{
					hSnd.Position = duccSnd.Follow.Transform.Position;
				}
				hSnd.Update();
			}
		}

		private static float GetEffectiveVolume( DuccSoundHandle sound )
		{
			return sound.Volume * GlobalSoundVolume;
		}

		private static float GetEffectivePitch( DuccSoundHandle sound )
		{
			return sound.Pitch * Math.Max( GameManager.ActiveScene.TimeScale, 0.3f );
		}
	}

	public class DuccSoundHandle
	{
		public DuccSoundHandle( SoundHandle hSnd )
		{
			_hSnd = hSnd;
			_volume = hSnd.Volume;
			_pitch = hSnd.Pitch;
		}

		public float Volume
		{
			get => _volume;
			set
			{
				_volume = value;
				_hSnd.Volume = value;
			}
		}
		private float _volume = 1f;
		public float Pitch
		{
			get => _pitch;
			set
			{
				_pitch = value;
				_hSnd.Pitch = value;
			}
		}
		private float _pitch = 1f;
		public Vector3 Position
		{
			get => _hSnd.Position;
			set => _hSnd.Position = value;
		}
		public bool ListenLocal
		{
			get => _hSnd.ListenLocal;
			set => _hSnd.ListenLocal = value;
		}
		public GameObject Follow { get; set; }
		
		public bool IsPlaying => _hSnd.IsPlaying;
		public bool IsStopped => _hSnd.IsStopped;

		public void Stop( float fadeTime = 0f )
		{
			_hSnd.Stop( fadeTime );
		}

		private SoundHandle _hSnd;
	}
}

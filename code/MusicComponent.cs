namespace Sandbox;

public sealed class MusicComponent : Component
{
	[ConVar("volume_music")]
	public static float GlobalMusicVolume { get; set; } = 1.0f;

	[Property] public string FilePath { get; set; }
	[Property] public bool PlayOnStart { get; set; }
	[Property] public bool Loop 
	{
		get => _loop;
		set
		{
			_loop = value;
			if ( _player is not null )
			{
				_player.Repeat = value;
			}
		}
	}
	private bool _loop;
	[Property, Range( 0f, 1f)] public float Volume { get; set; } = 1f;

	private MusicPlayer _player;

	protected override void OnStart()
	{
		if ( PlayOnStart )
		{
			Play();
		}
	}

	protected override void OnUpdate()
	{
		if ( _player is not null )
		{
			_player.Volume = Volume * GlobalMusicVolume;
		}
	}

	private void Play()
	{
		if ( !FileSystem.Mounted.FileExists( FilePath ) )
		{ 
			Log.Info( "Unable to find music file: " + FilePath );
			return;
		}
		Stop();
		_player = MusicPlayer.Play( FileSystem.Mounted, FilePath );
		_player.Volume = Volume * GlobalMusicVolume;
		_player.ListenLocal = true;
		_player.Repeat = Loop;
	}

	private void Stop()
	{
		_player?.Stop();
		_player?.Dispose();
		_player = null;
	}
}

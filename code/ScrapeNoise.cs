using Sandbox;
using System;

public sealed class ScrapeNoise : Component
{
	[Property] public float VolumeFactor { get; set; } = 1f;
	[Property] public SoundEvent ScrapeSound { get; set; }
	[Property] public CharacterController Character { get; set; }

	private SoundHandle _currentSound;

	protected override void OnUpdate()
	{
		if ( !ShouldPlaySound() )
		{
			StopSound();
			return;
		}

		EnsureSoundStarted();
		UpdateSound();
	}

	private bool ShouldPlaySound()
	{
		return ScrapeSound is not null 
			&& Character.IsValid() 
			&& !Character.Velocity.IsNearZeroLength;
	}

	private void StopSound()
	{
		_currentSound?.Stop( 0.2f );
		_currentSound = null;
	}

	private void EnsureSoundStarted()
	{
		if ( _currentSound is null || _currentSound.IsStopped )
		{
			_currentSound = Sound.Play( ScrapeSound );
			// The pitch will reset on each loop. Is this okay?
			_currentSound.Pitch = 0f;
		}
	}

	private void UpdateSound()
	{
		var intensity = GetScrapeIntensity();
		_currentSound.Volume = GetScrapeIntensity() * VolumeFactor;
		// Never reduce the pitch of the scrape sound once it starts.
		_currentSound.Pitch = MathF.Max( _currentSound.Pitch, intensity );
		_currentSound.Position = Transform.Position;
	}

	private float GetScrapeIntensity()
	{
		var intensity = 1f;
		if ( Character.IsValid() )
		{
			intensity = MathX.LerpInverse( Character.Velocity.Length, 0f, 200f, false );
		}
		return intensity;
	}
}

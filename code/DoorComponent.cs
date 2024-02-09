using Sandbox;
using System;
using System.Linq;

public sealed class DoorComponent : Component
{
	public enum DoorState
	{
		Closed,
		Opening,
		Open,
		Closing
	}

	[Property] public Action OnOpening { get; set; }
	[Property] public Action OnOpened { get; set; }
	[Property] public Action OnClosing { get; set; }
	[Property] public Action OnClosed { get; set; }

	[Property] public string DoorName { get; set; }
	[Property] public GameObject Shutter { get; set; }
	[Property] public GameObject OpenTarget { get; set; }
	[Property] public GameObject CloseTarget { get; set; }
	[Property] public Light DoorLight { get; set; }
	[Property] public float DoorSpeed { get; set; } = 5f;
	[Property] public Color ClosedLightColor { get; set; } = Color.Red;
	[Property] public Color OpenedLightColor { get; set; } = Color.Blue;
	[Property] public bool ShouldBeOpen
	{
		get => _state == DoorState.Open || _state == DoorState.Opening;
		set
		{
			if ( value )
			{
				Open();
			}
			else
			{
				Close();
			}
		}
	}
	[Property] public DoorState State => _state;
	private DoorState _state;

	protected override void OnStart()
	{
		if ( ShouldBeOpen )
		{
			Open();
		}
		else
		{
			Close();
		}
	}


	protected override void OnUpdate()
	{
		var target = _state switch
		{
			DoorState.Opening => OpenTarget,
			DoorState.Closing => CloseTarget,
			_ => null
		};
		if ( target is null )
			return;

		var startPos = Shutter.Transform.Position;
		var endPos = target.Transform.Position;
		Shutter.Transform.Position = startPos.LerpTo( endPos, Time.Delta * 5f );
		var distance = Shutter.Transform.Position.Distance( endPos );
		if ( distance <= 0.5f )
		{
			Shutter.Transform.Position = endPos;
			switch ( _state )
			{
				case DoorState.Opening:
					_state = DoorState.Open;
					OnOpened?.Invoke();
					break;
				case DoorState.Closing:
					_state = DoorState.Closed;
					OnClosed?.Invoke();
					break;
			}
		}
	}

	public void Open()
	{
		if ( _state == DoorState.Open )
			return;

		if ( DoorLight.IsValid() )
		{
			DoorLight.LightColor = OpenedLightColor;
		}
		if ( GameManager.IsPlaying )
		{
			_state = DoorState.Opening;
			OnOpening?.Invoke();
		}
		else
		{
			_state = DoorState.Open;
			if ( Shutter.IsValid() && OpenTarget.IsValid() )
			{
				Shutter.Transform.Position = OpenTarget.Transform.Position;
			}
		}
	}

	public void Close()
	{
		if ( _state == DoorState.Closed )
			return;

		if ( DoorLight.IsValid() )
		{
			DoorLight.LightColor = ClosedLightColor;
		}
		if ( GameManager.IsPlaying )
		{
			_state = DoorState.Closing;
			OnClosing?.Invoke();
		}
		else
		{
			_state = DoorState.Closed;
			if ( Shutter.IsValid() && CloseTarget.IsValid() )
			{
				Shutter.Transform.Position = CloseTarget.Transform.Position;
			}
		}
	}

	[ActionGraphNode( "door.find")]
	[Title( "Find Door" ), Group( "Door" )]
	public static DoorComponent Find( string doorName )
	{
		return GameManager.ActiveScene
			.GetAllComponents<DoorComponent>()
			.FirstOrDefault( d => d.DoorName.ToLower() == doorName.ToLower() );
	}
}

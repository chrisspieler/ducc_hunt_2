using System;
using System.Threading.Tasks;
using Sandbox;

public sealed class GameUI : Component
{
	[Property] public ModalPanel Modal { get; set; }
	[Property] public MegaToastPanel MegaToast { get; set; }

	public static GameUI Instance { get; private set; }

	public GameUI()
	{
		Instance = this;
	}

	[ActionGraphNode( "menu.showmodal" )]
	[Title( "Show Modal" ), Group( "Menu" )]
	public static void ShowModal( string message, string title = "" )
	{
		ShowModal( message, title, null, new string[] { "Ok" } );
	}

	[ActionGraphNode( "menu.showmodalchoices" )]
	[Title( "Show Modal with Choices" ), Group( "Menu" )]
	public static async Task<string> ShowModal( string message, string title, params string[] choices )
	{
		string choice = null;
		ShowModal( message, title, c => choice = c, choices );
		while ( choice is null )
		{
			await System.Threading.Tasks.Task.Delay( 100 );
		}
		return choice;
	}

	public static void ShowModal( string message, string title = "", Action<string> onClose = null )
	{
		ShowModal( message, title, onClose, "Ok" );
	}

	public static void ShowModal( string message, string title, Action<string> onClose, params string[] choices )
	{
		var modal = Instance.Modal;
		modal.Show( message, title, onClose, choices );
	}

	[ActionGraphNode( "menu.megatoast" )]
	[Title( "Show MegaToast" ), Group( "Menu" )]
	public static void ShowMegaToast( string message, float duration = 3.0f )
	{
		var megaToast = Instance.MegaToast;
		megaToast.Show( message, duration );
	}
}

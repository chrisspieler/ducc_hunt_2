using System;
using System.Threading.Tasks;
using Sandbox;

public sealed class GameUI : Component
{
	[Property] public ModalPanel Modal { get; set; }
	[Property] public MegaToastPanel MegaToast { get; set; }
	[Property] public BossHealthPanel BossPanel { get; set; }
	[Property] public VictoryPanel Victory { get; set; }
	[Property] public PictureInPicturePanel CameraFeed { get; set; }

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

	[ActionGraphNode( "menu.showbosspanel")]
	[Title( "Show Boss Panel" ), Group( "Menu" )]
	public static void ShowBossPanel( string bossName, float healthPercent )
	{
		var bossPanel = Instance.BossPanel;
		bossPanel.BossName = bossName;
		bossPanel.HealthPercent = healthPercent;
	}

	[ActionGraphNode( "menu.hidebosspanel")]
	[Title( "Close Boss Panel" ), Group( "Menu" )]
	public static void CloseBossPanel()
	{
		var bossPanel = Instance.BossPanel;
		bossPanel.BossName = null;
	}

	[ActionGraphNode( "menu.showvictorypanel" )]
	[Title( "Show Victory Message" ), Group( "Menu" )]
	public static void ShowVictoryPanel( string text, VictoryPanel.VictoryTextColor color )
	{
		var victory = Instance.Victory;
		victory.VictoryText = text;
		victory.TextColor = color;
		victory.Enabled = true;
	}

	[ActionGraphNode( "menu.setcamerafeed" )]
	[Title( "Set Camera Feed" ), Group( "Menu" )]
	public static void SetCameraFeed( CameraComponent camera )
	{
		var cameraFeed = Instance.CameraFeed;
		cameraFeed.ActiveCamera = camera;
	}
}

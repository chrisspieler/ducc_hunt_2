namespace Sandbox;

public static class DebugVars
{
	[ConVar("ai_debug")]
	public static bool AI { get; set; } = false;
}

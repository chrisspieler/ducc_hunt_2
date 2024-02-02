namespace Sandbox;

[GameResource("Pickup", "pickup", "An item which may be picked up by the player.")]
public class PickupData : GameResource
{
	public PrefabFile PickupPrefab { get; set; }
	public string Name { get; set; } = "New Item";
	public string Description { get; set; } = "What are you looking at?";
	public Color PickupTint { get; set; } = Color.Cyan;
	public SoundEvent PickupSound { get; set; }
}

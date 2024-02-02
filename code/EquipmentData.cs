namespace Sandbox;

[GameResource("Equipment", "equip", "An equippable item displayed on the person.")]
public class EquipmentData : PickupData
{
	public PrefabFile BodyPrefab { get; set; }
	public EquipmentSlot Slot { get; set; }
}

public enum EquipmentSlot
{
	Weapon,
	Head
}

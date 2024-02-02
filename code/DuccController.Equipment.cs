using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.Diagnostics;

public partial class DuccController
{
	public delegate void EquipmentAddedDelegate( EquipmentData equipment );

	[Property] public EquipmentAddedDelegate OnEquipmentAdded { get; set; }

	
	[Property, Category( "Hold Points" )] 
	public GameObject WeaponHoldPoint { get; set; }
	[Property, Category( "Hold Points" )] 
	public GameObject HeadHoldPoint { get; set; }

	private List<EquipmentData> _equipmentList = new();
	private Dictionary<EquipmentSlot, Equipment> _equipmentInstances = new();

	public void WearEquipment( EquipmentData equipment )
	{
		if ( !HasEquipment( equipment ) )
		{
			AddEquipment( equipment, false );
		}

		if ( IsEquipmentSlotOccupied( equipment.Slot ) )
		{
			// The equipment is responsible for cleaning itself up.
			_equipmentInstances[equipment.Slot].OnUnequip();
		}
		MakeEquipmentInstance( equipment );
	}

	private void MakeEquipmentInstance( EquipmentData equipment )
	{
		var prefabScene = SceneUtility.GetPrefabScene( equipment.BodyPrefab );
		var equipmentInstance = prefabScene.Clone();
		equipmentInstance.BreakFromPrefab();
		AddToHoldPoint( equipmentInstance, equipment.Slot );
		var equipmentComponent = equipmentInstance.Components.Get<Equipment>();
		Assert.NotNull( equipmentComponent );
		_equipmentInstances[equipment.Slot] = equipmentComponent;
	}

	private void AddToHoldPoint( GameObject go, EquipmentSlot slot )
	{
		var holdPoint = slot switch
		{
			EquipmentSlot.Head => HeadHoldPoint,
			EquipmentSlot.Weapon => WeaponHoldPoint,
			_ => throw new ArgumentOutOfRangeException( nameof( slot ) ),
		};
		go.Parent = holdPoint;
		go.Transform.World = holdPoint.Transform.World;
	}

	[ActionGraphNode( "ducc.addequipment" )]
	[Title( "Add Equipment" ), Group( "Ducc" )]
	public static void AddEquipment( EquipmentData equipment, bool wearOnPickup = true )
	{
		if ( !HasEquipment( equipment ) )
		{
			Instance._equipmentList.Add( equipment );
			Instance.OnEquipmentAdded?.Invoke( equipment );
		}

		if ( wearOnPickup )
		{
			Instance.WearEquipment( equipment );
		}
	}

	[ActionGraphNode( "ducc.hasequipment" )]
	[Title( "Has Equipment" ), Group( "Ducc" )]
	public static bool HasEquipment( EquipmentData equipment )
	{
		return Instance._equipmentList.Contains( equipment );
	}

	[ActionGraphNode( "ducc.equipmentslotoccupied" )]
	[Title ("Is Equipment Slot Occupied"), Group( "Ducc" )]
	public static bool IsEquipmentSlotOccupied( EquipmentSlot slot )
	{
		return Instance._equipmentInstances.ContainsKey( slot );
	}
}

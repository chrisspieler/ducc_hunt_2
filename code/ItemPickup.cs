using System;
using System.Linq;
using Sandbox;

public sealed class ItemPickup : Component, Component.ITriggerListener
{
	[Property] public Action<PickupData> OnPickup { get; set; }

	[Property] public PointLight RingLight { get; set; }
	[Property] public ModelRenderer RingModel { get; set; }
	[Property] public GameObject PickupPosition { get; set; }
	[Property] public PickupData Data { get; set; }
	[Property] public EquipmentData Equipment { get; set; }
	[Property] public bool Gravity { get; set; } = true;
	
	private GameObject _pickupInstance;

	protected override void OnStart()
	{
		// Workaround to lack of support for inheritance.
		Data = GetData();
		InitializePickupFromData();
	}

	private void InitializePickupFromData()
	{
		RingModel.Tint = Data.PickupTint.WithAlpha( 0.4f );
		var pickupParent = PickupPosition ?? GameObject;
		var prefabScene = SceneUtility.GetPrefabScene( Data.PickupPrefab );
		_pickupInstance = prefabScene.Clone();
		_pickupInstance.Parent = pickupParent;
		_pickupInstance.Transform.World = pickupParent.Transform.World;
	}

	private PickupData GetData()
	{
		return Equipment
			?? Data
			?? ResourceLibrary.GetAll<PickupData>().First();
	}

	protected override void OnUpdate()
	{
		UpdateRingLightPosition();
		if ( Gravity )
		{
			UpdateGravity();
		}
	}

	private void UpdateRingLightPosition()
	{
		var camTx = Scene.Camera.Transform;
		var modelTx = _pickupInstance.Transform;
		var dirToCam = (camTx.Position - modelTx.Position).Normal;
		RingLight.Transform.Position = modelTx.Position 
			+ dirToCam * 10f
			+ Vector3.Up * 15f;
	}

	private void UpdateGravity()
	{
		var ray = new Ray( Transform.Position, Vector3.Down );
		var tr = Scene.Trace
			.Ray( ray, 300f )
			.Size( 5f )
			.Run();
		if ( tr.StartedSolid )
			return;
		Transform.Position += Vector3.Down * tr.Distance * Time.Delta;
	}

	public bool CanPickup( GameObject player )
	{
		return player.Tags.Has( "player" );
	}
	public void OnTriggerEnter( Collider other )
	{
		if ( !CanPickup( other.GameObject ) )
			return;

		Pickup( other.GameObject );
	}

	public void Pickup( GameObject player )
	{
		Log.Info( $"{player.Name} picked up {Data.Name}" );
		OnPickup?.Invoke( Data );
		if ( Data.PickupSound is not null )
		{
			var hSnd = DuccSound.Play( Data.PickupSound );
			hSnd.ListenLocal = true;
		}
		if ( Data is EquipmentData equipment )
		{
			DuccController.AddEquipment( equipment );
		}
		GameObject.Destroy();
	}

	public void OnTriggerExit( Collider other ) { }

	[ActionGraphNode( "item.spawnpickup" )]
	[Title( "Spawn Pickup" ), Category( "Items" )]
	public static ItemPickup Spawn( PickupData data, Vector3 position )
	{
		var prefabFile = ResourceLibrary.Get<PrefabFile>( "prefabs/pickup.prefab" );
		var prefabScene = SceneUtility.GetPrefabScene( prefabFile );
		var pickupGo = prefabScene.Clone( position );
		var pickup = pickupGo.Components.Get<ItemPickup>();
		pickup.Data = data;
		return pickup;
	}

	[ActionGraphNode( "item.spawnequipment" )]
	[Title( "Spawn Equipment" ), Category( "Items" )]
	public static ItemPickup Spawn( EquipmentData data, Vector3 position )
	{
		// TODO: Remove this method once polymorphism is supported.
		return Spawn( data as PickupData, position );
	}
}

using Sandbox;

public sealed partial class DuccController : Component
{
	[Property] public CharacterController Character { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public float Speed { get; set; }

	[Property] public DistractionSource Distraction { get; set; }

	public Vector3 WishVelocity { get; set; }

	public static DuccController Instance { get; private set; }

	protected override void OnStart()
	{
		Character ??= Components.GetOrCreate<CharacterController>();
		Instance = this;
	}

	protected override void OnUpdate()
	{
		UpdateQuack();
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
		{
			Instance = null;
		}
	}

	private void UpdateQuack()
	{
		if ( Input.Pressed( "attack2" ) )
		{
			Distraction.DeployDistraction();
		}
	}

	protected override void OnFixedUpdate()
	{
		UpdateWishVelocity();
		if ( !Character.IsOnGround )
		{
			Character.Velocity += Vector3.Down * 50f;
		}
		Character.Accelerate( WishVelocity );
		Character.ApplyFriction( 4.0f, 150f );
		Character.Move();
		
		if ( !Character.Velocity.IsNearlyZero( 60f ) )
		{
			Body.Transform.Rotation = Rotation.LookAt( Character.Velocity.WithZ( 0f ).Normal, Vector3.Up );
		}
	}

	private void UpdateWishVelocity()
	{
		var inputLength = Input.AnalogMove.ClampLength( 0f, 1f).Length;
		var eyeRotation = Scene.Camera.Transform.Rotation;
		var inputDir = (Input.AnalogMove * eyeRotation).WithZ(0f).Normal;
		var inputVec = inputDir * inputLength;
		WishVelocity = inputVec * Speed;
	}
}

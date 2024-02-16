using Sandbox;

public sealed partial class DuccController : Component
{
	[Property] public CharacterController Character { get; set; }
	[Property] public GameObject Eye { get; set; }
	[Property] public GameObject Body { get; set; }
	[Property] public float Speed { get; set; } = 80f;

	[Property] public DistractionSource Distraction { get; set; }

	private Vector3 LastDirectionInput { get; set; }
	public Vector3 WishVelocity { get; set; }

	public static DuccController Instance { get; private set; }

	protected override void OnStart()
	{
		Character ??= Components.GetOrCreate<CharacterController>();
		Body ??= GameObject;
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


		var targetRotation = Rotation.LookAt( LastDirectionInput.WithZ( 0f ).Normal, Vector3.Up );
		Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetRotation, Time.Delta * 20f );

		if ( Transform.Position.z < -200f )
		{
			Transform.Position = Vector3.Up * 50f;
			Character.Velocity = Vector3.Zero;
		}
	}

	private void UpdateWishVelocity()
	{
		var inputLength = Input.AnalogMove.ClampLength( 0f, 1f).Length;
		var eyeRotation = Scene.Camera.Transform.Rotation;
		var inputDir = (Input.AnalogMove * eyeRotation).WithZ(0f).Normal;
		if ( !Input.AnalogMove.IsNearlyZero() )
		{
			LastDirectionInput = inputDir;
		}
		var inputVec = inputDir * inputLength;
		var speedFactor = Input.Down( "run" ) ? 1.5f : 1.0f;
		WishVelocity = inputVec * Speed * speedFactor;
	}
}

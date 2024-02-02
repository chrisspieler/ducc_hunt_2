namespace Sandbox;

public sealed class CCTVComponent : Component
{
	[Property] public CameraComponent Camera { get; set; }
	[Property] public DynamicTextureComponent Target { get; set; }
	[Property] public Vector2 RenderSize { get; set; } = new Vector2( 256, 256 );

	private Texture _renderTarget;
	protected override void OnUpdate()
	{
		if ( Target.InputTexture is null || Target.InputTexture.Size != RenderSize )
		{
			Target.InitializeTexture( RenderSize );
		}
		_renderTarget ??= Texture.CreateRenderTarget()
				.WithSize( RenderSize )
				.Create();
		if ( Target.InputTexture != _renderTarget )
		{
			Target.SetTexture( _renderTarget );
		}
		Camera.RenderToTexture( _renderTarget );
	}
}

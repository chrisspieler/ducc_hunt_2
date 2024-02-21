using Sandbox;

public sealed class Trigger : Component, Component.ITriggerListener
{
	public delegate void TriggerDelegate( Collider other );

	[Property] public TriggerDelegate OnEnter { get; set; }
	[Property] public TriggerDelegate OnExit { get; set; }


	[Property, Category( "Filter" )]
	public TagSet IncludeAny { get; set; }
	[Property, Category( "Filter" )]
	public TagSet ExcludeAny { get; set; }

	public void OnTriggerEnter( Collider other )
	{
		if ( !CanTriggerFrom( other ) )
			return;

		OnEnter?.Invoke( other );
	}

	public void OnTriggerExit( Collider other )
	{
		if ( !CanTriggerFrom( other ) )
			return;

		OnExit?.Invoke( other );
	}

	private bool CanTriggerFrom( Collider other )
	{
		return ( IncludeAny is null || IncludeAny.IsEmpty || other.Tags.HasAny( IncludeAny ) )
			&& ( ExcludeAny is null || ExcludeAny.IsEmpty || !other.Tags.HasAny( ExcludeAny ));
	}
}

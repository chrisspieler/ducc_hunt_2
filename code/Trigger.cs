using Sandbox;
using System.Linq;

public sealed class Trigger : Component, Component.ITriggerListener
{
	public delegate void TriggerDelegate( Collider other );

	[Property] public TriggerDelegate OnEnter { get; set; }
	[Property] public TriggerDelegate OnExit { get; set; }


	[Property, Category( "Filter" )]
	public TagSet IncludeAny { get; set; } = new();
	[Property, Category( "Filter" )]
	public TagSet ExcludeAny { get; set; } = new();

	public void OnTriggerEnter( Collider other )
	{
		if ( !Filter( other ) )
			return;

		OnEnter?.Invoke( other );
	}

	public void OnTriggerExit( Collider other )
	{
		if ( !Filter( other ) )
			return;

		OnExit?.Invoke( other );
	}

	private bool Filter( Collider other )
	{
		return ( !IncludeAny.TryGetAll().Any() || other.Tags.HasAny( IncludeAny ) )
			&& !other.Tags.HasAny( ExcludeAny );
	}
}

namespace Ducc.AI.Flow
{
	public class HasKeyConditional : BehaviorNode
	{
		public string Key { get; set; }

		protected override BehaviorResult ExecuteInternal( ActorComponent actor, DataContext context )
		{
			return context.HasKey( Key )
				? BehaviorResult.Success
				: BehaviorResult.Failure;
		}
	}
}

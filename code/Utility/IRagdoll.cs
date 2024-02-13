using System;

namespace Sandbox
{
	public interface IRagdoll
	{
		Action OnRagdollStart { get; set; }
		Action OnRagdollEnd { get; set; }
		bool IsRagdoll { get; }
		void SetRagdollState( bool isRagdoll );
	}
}

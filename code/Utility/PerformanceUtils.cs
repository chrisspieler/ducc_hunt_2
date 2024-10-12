using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Utility
{
	public static class PerformanceUtils
	{
		[ActionGraphNode("perf.disableshadows")]
		[Title("Disable Shadows"), Group("Performance")]
		public static void DisableShadows()
		{
			if ( !Game.ActiveScene.IsValid() )
				return;

			var lights = Game.ActiveScene
				.GetAllComponents<Light>();
			foreach ( var light in lights )
			{
				light.Shadows = false;
			}
		}
	}
}

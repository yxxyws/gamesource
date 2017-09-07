using System;
namespace RimWorld
{
	public static class RaidStrategyDefOf
	{
		public static RaidStrategyDef ImmediateAttack;
		public static void RebindDefs()
		{
			DefOfHelper.BindDefsFor<RaidStrategyDef>(typeof(RaidStrategyDefOf));
		}
	}
}

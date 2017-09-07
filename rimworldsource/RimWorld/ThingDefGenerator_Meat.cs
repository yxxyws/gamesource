using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public static class ThingDefGenerator_Meat
	{
		[DebuggerHidden]
		public static IEnumerable<ThingDef> ImpliedMeatDefs()
		{
			ThingDefGenerator_Meat.<ImpliedMeatDefs>c__Iterator60 <ImpliedMeatDefs>c__Iterator = new ThingDefGenerator_Meat.<ImpliedMeatDefs>c__Iterator60();
			ThingDefGenerator_Meat.<ImpliedMeatDefs>c__Iterator60 expr_07 = <ImpliedMeatDefs>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
		private static float GetMeatMarketValue(ThingDef sourceDef)
		{
			if (sourceDef.race.Humanlike)
			{
				return 1.75f;
			}
			return 3.5f;
		}
	}
}

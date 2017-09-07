using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
namespace RimWorld
{
	public static class ThingDefGenerator_Leather
	{
		private const float HumanlikeLeatherCommonalityFactor = 0.01f;
		private static bool GeneratesLeather(ThingDef sourceDef)
		{
			return sourceDef.category == ThingCategory.Pawn && sourceDef.GetStatValueAbstract(StatDefOf.LeatherAmount, null) > 0f;
		}
		[DebuggerHidden]
		public static IEnumerable<ThingDef> ImpliedLeatherDefs()
		{
			ThingDefGenerator_Leather.<ImpliedLeatherDefs>c__Iterator5F <ImpliedLeatherDefs>c__Iterator5F = new ThingDefGenerator_Leather.<ImpliedLeatherDefs>c__Iterator5F();
			ThingDefGenerator_Leather.<ImpliedLeatherDefs>c__Iterator5F expr_07 = <ImpliedLeatherDefs>c__Iterator5F;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}

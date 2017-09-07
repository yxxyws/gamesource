using System;
using Verse;
namespace RimWorld
{
	public static class DrugUtility
	{
		public static float DrunknessPercent(Pawn pawn)
		{
			Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Alcohol);
			if (firstHediffOfDef != null)
			{
				return firstHediffOfDef.Severity;
			}
			return 0f;
		}
	}
}

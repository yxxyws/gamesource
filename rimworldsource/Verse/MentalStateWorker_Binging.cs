using RimWorld;
using System;
namespace Verse
{
	public class MentalStateWorker_Binging : MentalStateWorker
	{
		public override bool StateCanOccur(Pawn pawn)
		{
			return pawn.Faction == Faction.OfColony && pawn.GetPosture() == PawnPosture.Standing && Find.ListerThings.ThingsOfDef(ThingDefOf.Beer).Count > 0;
		}
	}
}

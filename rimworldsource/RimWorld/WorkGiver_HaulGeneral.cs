using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_HaulGeneral : WorkGiver_Haul
	{
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return ListerHaulables.ThingsPotentiallyNeedingHauling();
		}
		public override bool ShouldSkip(Pawn pawn)
		{
			return ListerHaulables.ThingsPotentiallyNeedingHauling().Count == 0;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			if (t is Corpse)
			{
				return null;
			}
			if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t))
			{
				return null;
			}
			return HaulAIUtility.HaulToStorageJob(pawn, t);
		}
	}
}

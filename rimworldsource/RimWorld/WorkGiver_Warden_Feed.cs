using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Warden_Feed : WorkGiver_Warden
	{
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			if (!base.ShouldTakeCareOfPrisoner(pawn, t))
			{
				return null;
			}
			Pawn pawn2 = (Pawn)t;
			if (!WardenFeedUtility.ShouldBeFed(pawn2))
			{
				return null;
			}
			if (pawn2.needs.food.CurLevelPercentage >= pawn2.needs.food.PercentageThreshHungry + 0.02f)
			{
				return null;
			}
			ThingDef def;
			Thing thing = FoodUtility.BestFoodSourceFor(pawn, pawn2, true, out def);
			if (thing == null)
			{
				return null;
			}
			return new Job(JobDefOf.FeedPatient, thing, pawn2)
			{
				maxNumToCarry = FoodUtility.WillEatStackCountOf(pawn2, def)
			};
		}
	}
}

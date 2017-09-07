using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_FeedPatient : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForGroup(ThingRequestGroup.Pawn);
			}
		}
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.Touch;
			}
		}
		public override bool HasJobOnThing(Pawn pawn, Thing t)
		{
			Pawn pawn2 = t as Pawn;
			ThingDef thingDef;
			return pawn2 != null && pawn2 != pawn && FeedPatientUtility.ShouldBeFed(pawn2) && pawn2.needs.food.CurLevelPercentage <= pawn2.needs.food.PercentageThreshHungry + 0.02f && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, Danger.Deadly, 1) && FoodUtility.BestFoodSourceFor(pawn, pawn2, true, out thingDef) != null;
		}
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			Pawn pawn2 = (Pawn)t;
			ThingDef def;
			Thing t2 = FoodUtility.BestFoodSourceFor(pawn, pawn2, true, out def);
			return new Job(JobDefOf.FeedPatient)
			{
				targetA = t2,
				targetB = pawn2,
				maxNumToCarry = FoodUtility.WillEatStackCountOf(pawn2, def)
			};
		}
	}
}

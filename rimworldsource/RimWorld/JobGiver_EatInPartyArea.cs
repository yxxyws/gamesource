using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_EatInPartyArea : ThinkNode_JobGiver
	{
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			PawnDuty duty = pawn.mindState.duty;
			if (duty == null)
			{
				return null;
			}
			float curLevelPercentage = pawn.needs.food.CurLevelPercentage;
			if ((double)curLevelPercentage > 0.9)
			{
				return null;
			}
			IntVec3 cell = duty.focus.Cell;
			Thing thing = this.FindFood(pawn, cell);
			if (thing == null)
			{
				return null;
			}
			return new Job(JobDefOf.Ingest, thing)
			{
				maxNumToCarry = FoodUtility.WillEatStackCountOf(pawn, thing.def)
			};
		}
		private Thing FindFood(Pawn pawn, IntVec3 partySpot)
		{
			Predicate<Thing> validator = (Thing x) => x.IngestibleNow && PartyUtility.InPartyArea(x.Position, partySpot) && !x.def.ingestible.isPleasureDrug && x.def.ingestible.preferability > FoodPreferability.Raw && pawn.RaceProps.WillAutomaticallyEat(x) && !x.IsForbidden(pawn) && x.IsSociallyProper(pawn) && pawn.CanReserve(x, 1);
			return GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForGroup(ThingRequestGroup.FoodNotPlantOrTree), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), 14f, validator, null, 12, false);
		}
	}
}

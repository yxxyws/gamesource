using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class WorkGiver_Warden_DeliverFood : WorkGiver_Warden
	{
		public override Job JobOnThing(Pawn pawn, Thing t)
		{
			if (!base.ShouldTakeCareOfPrisoner(pawn, t))
			{
				return null;
			}
			Pawn pawn2 = (Pawn)t;
			if (!pawn2.guest.ShouldBeBroughtFood)
			{
				return null;
			}
			if (pawn2.needs.food.CurLevelPercentage >= pawn2.needs.food.PercentageThreshHungry + 0.02f)
			{
				return null;
			}
			if (WardenFeedUtility.ShouldBeFed(pawn2))
			{
				return null;
			}
			ThingDef def;
			Thing thing = FoodUtility.BestFoodSourceFor(pawn, pawn2, true, out def);
			if (thing == null)
			{
				return null;
			}
			if (thing.GetRoom() == pawn2.GetRoom())
			{
				return null;
			}
			if (WorkGiver_Warden_DeliverFood.FoodAvailableInRoomTo(pawn2))
			{
				return null;
			}
			return new Job(JobDefOf.DeliverFood, thing, pawn2)
			{
				maxNumToCarry = FoodUtility.WillEatStackCountOf(pawn2, def),
				targetC = RCellFinder.SpotToChewStandingNear(pawn2, thing)
			};
		}
		private static bool FoodAvailableInRoomTo(Pawn prisoner)
		{
			if (prisoner.carrier.CarriedThing != null && FoodUtility.NutritionAvailableFromFor(prisoner.carrier.CarriedThing, prisoner) > 0f)
			{
				return true;
			}
			float num = 0f;
			float num2 = 0f;
			Room room = RoomQuery.RoomAt(prisoner.Position);
			if (room == null)
			{
				return false;
			}
			for (int i = 0; i < room.RegionCount; i++)
			{
				Region region = room.Regions[i];
				List<Thing> list = region.ListerThings.ThingsInGroup(ThingRequestGroup.FoodNotPlantOrTree);
				for (int j = 0; j < list.Count; j++)
				{
					Thing thing = list[j];
					if (thing.def.ingestible.preferability > FoodPreferability.NeverForNutrition)
					{
						num2 += FoodUtility.NutritionAvailableFromFor(thing, prisoner);
					}
				}
				List<Thing> list2 = region.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
				for (int k = 0; k < list2.Count; k++)
				{
					Pawn pawn = list2[k] as Pawn;
					if (pawn.IsPrisonerOfColony && pawn.needs.food.CurLevelPercentage < pawn.needs.food.PercentageThreshHungry + 0.02f && (pawn.carrier.CarriedThing == null || !pawn.RaceProps.WillAutomaticallyEat(pawn.carrier.CarriedThing)))
					{
						num += pawn.needs.food.NutritionWanted;
					}
				}
			}
			return num2 + 0.5f >= num;
		}
	}
}

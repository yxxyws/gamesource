using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_GetFood : ThinkNode_JobGiver
	{
		public override float GetPriority(Pawn pawn)
		{
			Need_Food food = pawn.needs.food;
			if (food == null)
			{
				return 0f;
			}
			if (pawn.needs.food.CurCategory < HungerCategory.Starving && FoodUtility.ShouldBeFedBySomeone(pawn))
			{
				return 0f;
			}
			if (food.CurLevelPercentage < food.PercentageThreshHungry + 0.02f)
			{
				return 9.5f;
			}
			return 0f;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Thing thing = null;
			if (pawn.RaceProps.ToolUser)
			{
				thing = FoodUtility.FoodInInventory(pawn, null, FoodPreferability.Awful, FoodPreferability.Lavish, 0f);
				if (thing != null)
				{
					if (pawn.Faction != Faction.OfColony)
					{
						return this.IngestJob(pawn, thing);
					}
					CompRottable compRottable = thing.TryGetComp<CompRottable>();
					if (compRottable != null && compRottable.TicksUntilRotAtCurrentTemp < 30000)
					{
						return this.IngestJob(pawn, thing);
					}
				}
			}
			ThingDef def;
			Thing thing2 = FoodUtility.BestFoodSourceFor(pawn, pawn, false, out def);
			if (thing != null && (thing2 == null || !pawn.Position.InHorDistOf(thing2.Position, 50f)))
			{
				return this.IngestJob(pawn, thing);
			}
			if (thing2 == null)
			{
				return null;
			}
			Building_NutrientPasteDispenser building_NutrientPasteDispenser = thing2 as Building_NutrientPasteDispenser;
			if (building_NutrientPasteDispenser != null && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())
			{
				Building building = building_NutrientPasteDispenser.AdjacentReachableHopper(pawn);
				if (building == null)
				{
					thing2 = FoodUtility.BestFoodSpawnedFor(pawn, pawn, true, FoodPreferability.Lavish, true, true);
					if (thing2 == null)
					{
						return null;
					}
				}
				else
				{
					ISlotGroupParent hopperSgp = building as ISlotGroupParent;
					Job job = WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, hopperSgp);
					if (job != null)
					{
						return job;
					}
					thing2 = FoodUtility.BestFoodSpawnedFor(pawn, pawn, true, FoodPreferability.Lavish, true, true);
					if (thing2 == null)
					{
						return null;
					}
					def = thing2.def;
				}
			}
			Pawn pawn2 = thing2 as Pawn;
			if (pawn2 != null)
			{
				return new Job(JobDefOf.PredatorHunt, pawn2)
				{
					killIncappedTarget = true
				};
			}
			return new Job(JobDefOf.Ingest, thing2)
			{
				maxNumToCarry = FoodUtility.WillEatStackCountOf(pawn, def)
			};
		}
		private Job IngestJob(Pawn pawn, Thing food)
		{
			Job job = new Job(JobDefOf.Ingest, food);
			job.maxNumToCarry = FoodUtility.WillEatStackCountOf(pawn, food.def);
			if (pawn.inventory != null && pawn.inventory.container.Contains(food))
			{
				job.eatFromInventory = true;
			}
			return job;
		}
	}
}

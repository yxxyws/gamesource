using System;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class WorkGiver_InteractAnimal : WorkGiver_Scanner
	{
		protected static string NoUsableFoodTrans;
		protected static string AnimalInteractedTooRecentlyTrans;
		private static string AnimalsSkillTooLowTrans;
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.OnCell;
			}
		}
		public static void Reset()
		{
			WorkGiver_InteractAnimal.NoUsableFoodTrans = "NoUsableFood".Translate();
			WorkGiver_InteractAnimal.AnimalInteractedTooRecentlyTrans = "AnimalInteractedTooRecently".Translate();
			WorkGiver_InteractAnimal.AnimalsSkillTooLowTrans = "AnimalsSkillTooLow".Translate();
		}
		protected virtual bool CanInteractWithAnimal(Pawn pawn, Pawn animal)
		{
			if (!pawn.CanReserve(animal, 1))
			{
				return false;
			}
			if (animal.Downed)
			{
				return false;
			}
			if (!animal.CasualInterruptibleNow())
			{
				return false;
			}
			if (Mathf.RoundToInt(animal.GetStatValue(StatDefOf.MinimumHandlingSkill, true)) > pawn.skills.GetSkill(SkillDefOf.Animals).level)
			{
				JobFailReason.Is(WorkGiver_InteractAnimal.AnimalsSkillTooLowTrans);
				return false;
			}
			return true;
		}
		protected bool HasFoodToInteractAnimal(Pawn pawn, Pawn tamee)
		{
			ThingContainer container = pawn.inventory.container;
			int num = 0;
			float num2 = JobDriver_InteractAnimal.RequiredNutritionPerFeed(tamee);
			for (int i = 0; i < container.Count; i++)
			{
				float num3 = 0f;
				Thing thing = container[i];
				if (tamee.RaceProps.CanEverEat(thing) && thing.def.ingestible.preferability <= FoodPreferability.Raw)
				{
					for (int j = 0; j < thing.stackCount; j++)
					{
						num3 += thing.def.ingestible.nutrition;
						if (num3 >= num2)
						{
							num++;
							num3 = 0f;
						}
						if (num >= 2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		protected Job TakeFoodForAnimalInteractJob(Pawn pawn, Pawn tamee)
		{
			float num = JobDriver_InteractAnimal.RequiredNutritionPerFeed(tamee) * 2f * 4f;
			Thing thing = FoodUtility.BestFoodSpawnedFor(pawn, tamee, false, FoodPreferability.Raw, false, false);
			if (thing == null)
			{
				return null;
			}
			return new Job(JobDefOf.TakeInventory, thing)
			{
				maxNumToCarry = Mathf.CeilToInt(num / thing.def.ingestible.nutrition)
			};
		}
	}
}

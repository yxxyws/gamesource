using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorld
{
	public class Building_NutrientPasteDispenser : Building
	{
		public CompPowerTrader powerComp;
		private List<IntVec3> cachedAdjCellsCardinal;
		public static int CollectDuration = 50;
		public bool CanDispenseNow
		{
			get
			{
				return this.powerComp.PowerOn && this.HasEnoughFeedstockInHoppers();
			}
		}
		private List<IntVec3> AdjCellsCardinalInBounds
		{
			get
			{
				if (this.cachedAdjCellsCardinal == null)
				{
					this.cachedAdjCellsCardinal = (
						from c in GenAdj.CellsAdjacentCardinal(this)
						where c.InBounds()
						select c).ToList<IntVec3>();
				}
				return this.cachedAdjCellsCardinal;
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.powerComp = base.GetComp<CompPowerTrader>();
		}
		public Building AdjacentReachableHopper(Pawn reacher)
		{
			for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
			{
				IntVec3 c = this.AdjCellsCardinalInBounds[i];
				Building edifice = c.GetEdifice();
				if (edifice != null && edifice.def == ThingDefOf.Hopper && reacher.CanReach(edifice, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
				{
					return (Building_Storage)edifice;
				}
			}
			return null;
		}
		public Thing TryDispenseFood()
		{
			if (!this.CanDispenseNow)
			{
				return null;
			}
			int foodCostPerDispense = this.def.building.foodCostPerDispense;
			int num = 0;
			List<ThingDef> list = new List<ThingDef>();
			while (true)
			{
				Thing thing = this.FindFeedInAnyHopper();
				if (thing == null)
				{
					break;
				}
				int num2 = Mathf.Min(thing.stackCount, foodCostPerDispense);
				num += num2;
				list.Add(thing.def);
				thing.SplitOff(num2);
				if (num >= foodCostPerDispense)
				{
					goto Block_3;
				}
			}
			Log.Error("Did not find enough food in hoppers while trying to dispense.");
			return null;
			Block_3:
			this.def.building.soundDispense.PlayOneShot(base.Position);
			Meal meal = (Meal)ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste, null);
			for (int i = 0; i < list.Count; i++)
			{
				meal.RegisterIngredient(list[i]);
			}
			return meal;
		}
		private Thing FindFeedInAnyHopper()
		{
			for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
			{
				Thing thing = null;
				Thing thing2 = null;
				List<Thing> thingList = this.AdjCellsCardinalInBounds[i].GetThingList();
				for (int j = 0; j < thingList.Count; j++)
				{
					Thing thing3 = thingList[j];
					if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing3.def))
					{
						thing = thing3;
					}
					if (thing3.def == ThingDefOf.Hopper)
					{
						thing2 = thing3;
					}
				}
				if (thing != null && thing2 != null)
				{
					return thing;
				}
			}
			return null;
		}
		public bool HasEnoughFeedstockInHoppers()
		{
			int num = 0;
			for (int i = 0; i < this.AdjCellsCardinalInBounds.Count; i++)
			{
				IntVec3 c = this.AdjCellsCardinalInBounds[i];
				Thing thing = null;
				Thing thing2 = null;
				List<Thing> thingList = c.GetThingList();
				for (int j = 0; j < thingList.Count; j++)
				{
					Thing thing3 = thingList[j];
					if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing3.def))
					{
						thing = thing3;
					}
					if (thing3.def == ThingDefOf.Hopper)
					{
						thing2 = thing3;
					}
				}
				if (thing != null && thing2 != null)
				{
					num += thing.stackCount;
				}
				if (num >= this.def.building.foodCostPerDispense)
				{
					return true;
				}
			}
			return false;
		}
		public static bool IsAcceptableFeedstock(ThingDef def)
		{
			return def.IsNutritionSource && def.ingestible.preferability != FoodPreferability.Undefined && (def.ingestible.foodType & FoodTypeFlags.Plant) != FoodTypeFlags.Plant && (def.ingestible.foodType & FoodTypeFlags.Tree) != FoodTypeFlags.Tree;
		}
	}
}

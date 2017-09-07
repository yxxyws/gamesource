using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace RimWorld
{
	internal class WorkGiver_CookFillHopper : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				return ThingRequest.ForDef(ThingDef.Named("Hopper"));
			}
		}
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.ClosestTouch;
			}
		}
		public override Job JobOnThing(Pawn pawn, Thing thing)
		{
			ISlotGroupParent slotGroupParent = thing as ISlotGroupParent;
			if (slotGroupParent == null)
			{
				return null;
			}
			if (!pawn.CanReserve(thing.Position, 1))
			{
				return null;
			}
			int num = 0;
			List<Thing> list = Find.ThingGrid.ThingsListAt(thing.Position);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing2 = list[i];
				if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(thing2.def))
				{
					num += thing2.stackCount;
				}
			}
			if (num > 25)
			{
				JobFailReason.Is("AlreadyFilledLower".Translate());
				return null;
			}
			return WorkGiver_CookFillHopper.HopperFillFoodJob(pawn, slotGroupParent);
		}
		public static Job HopperFillFoodJob(Pawn pawn, ISlotGroupParent hopperSgp)
		{
			Building building = hopperSgp as Building;
			if (!pawn.CanReserveAndReach(building.Position, PathEndMode.Touch, pawn.NormalMaxDanger(), 1))
			{
				return null;
			}
			ThingDef thingDef = null;
			Thing firstItem = building.Position.GetFirstItem();
			if (firstItem != null)
			{
				if (Building_NutrientPasteDispenser.IsAcceptableFeedstock(firstItem.def))
				{
					thingDef = firstItem.def;
				}
				else
				{
					if (firstItem.IsForbidden(pawn))
					{
						return null;
					}
					return HaulAIUtility.HaulAsideJobFor(pawn, firstItem);
				}
			}
			List<Thing> list;
			if (thingDef == null)
			{
				list = Find.Map.listerThings.ThingsInGroup(ThingRequestGroup.FoodNotPlantOrTree);
			}
			else
			{
				list = Find.Map.listerThings.ThingsOfDef(thingDef);
			}
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing.def.ingestible.preferability == FoodPreferability.Raw)
				{
					if (HaulAIUtility.PawnCanAutomaticallyHaul(pawn, thing))
					{
						if (Find.SlotGroupManager.SlotGroupAt(building.Position).Settings.AllowedToAccept(thing))
						{
							StoragePriority storagePriority = HaulAIUtility.StoragePriorityAtFor(thing.Position, thing);
							if (storagePriority < hopperSgp.GetSlotGroup().Settings.Priority)
							{
								Job job = HaulAIUtility.HaulMaxNumToCellJob(pawn, thing, building.Position, true);
								if (job != null)
								{
									return job;
								}
							}
						}
					}
				}
			}
			return null;
		}
	}
}

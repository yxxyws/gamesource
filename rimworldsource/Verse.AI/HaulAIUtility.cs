using RimWorld;
using System;
namespace Verse.AI
{
	public static class HaulAIUtility
	{
		private static string ForbiddenLowerTrans;
		private static string PrisonerRoomLowerTrans;
		private static string BurningLowerTrans;
		private static string NoEmptyPlaceLowerTrans;
		public static void Reset()
		{
			HaulAIUtility.ForbiddenLowerTrans = "ForbiddenLower".Translate();
			HaulAIUtility.PrisonerRoomLowerTrans = "PrisonerRoomLower".Translate();
			HaulAIUtility.BurningLowerTrans = "BurningLower".Translate();
			HaulAIUtility.NoEmptyPlaceLowerTrans = "NoEmptyPlaceLower".Translate();
		}
		public static bool PawnCanAutomaticallyHaul(Pawn p, Thing t)
		{
			if (!t.def.EverHaulable)
			{
				return false;
			}
			if (t.IsForbidden(p))
			{
				JobFailReason.Is(HaulAIUtility.ForbiddenLowerTrans);
				return false;
			}
			UnfinishedThing unfinishedThing = t as UnfinishedThing;
			if (unfinishedThing != null && unfinishedThing.BoundBill != null)
			{
				return false;
			}
			if (!t.def.alwaysHaulable && Find.DesignationManager.DesignationOn(t, DesignationDefOf.Haul) == null && !t.IsInValidStorage())
			{
				return false;
			}
			if (!p.CanReserveAndReach(t, PathEndMode.ClosestTouch, p.NormalMaxDanger(), 1))
			{
				return false;
			}
			if (t.def.IsNutritionSource && t.def.ingestible.HumanEdible && !t.IsSociallyProper(p))
			{
				JobFailReason.Is(HaulAIUtility.PrisonerRoomLowerTrans);
				return false;
			}
			if (t.IsBurning())
			{
				JobFailReason.Is(HaulAIUtility.BurningLowerTrans);
				return false;
			}
			return true;
		}
		public static bool PawnCanAutomaticallyHaulFast(Pawn p, Thing t)
		{
			UnfinishedThing unfinishedThing = t as UnfinishedThing;
			if (unfinishedThing != null && unfinishedThing.BoundBill != null)
			{
				return false;
			}
			if (!p.CanReach(t, PathEndMode.ClosestTouch, p.NormalMaxDanger(), false, TraverseMode.ByPawn))
			{
				return false;
			}
			if (!Find.Reservations.CanReserve(p, t, 1))
			{
				return false;
			}
			if (t.def.IsNutritionSource && t.def.ingestible.HumanEdible && !t.IsSociallyProper(p, false, true))
			{
				JobFailReason.Is(HaulAIUtility.PrisonerRoomLowerTrans);
				return false;
			}
			if (t.IsBurning())
			{
				JobFailReason.Is(HaulAIUtility.BurningLowerTrans);
				return false;
			}
			return true;
		}
		public static Job HaulToStorageJob(Pawn p, Thing t)
		{
			StoragePriority currentPriority = HaulAIUtility.StoragePriorityAtFor(t.Position, t);
			IntVec3 storeCell;
			if (!StoreUtility.TryFindBestBetterStoreCellFor(t, p, currentPriority, p.Faction, out storeCell, true))
			{
				JobFailReason.Is(HaulAIUtility.NoEmptyPlaceLowerTrans);
				return null;
			}
			return HaulAIUtility.HaulMaxNumToCellJob(p, t, storeCell, false);
		}
		public static Job HaulMaxNumToCellJob(Pawn p, Thing t, IntVec3 storeCell, bool fitInStoreCell)
		{
			Job job = new Job(JobDefOf.HaulToCell, t, storeCell);
			if (Find.SlotGroupManager.SlotGroupAt(storeCell) != null)
			{
				Thing thing = Find.ThingGrid.ThingAt(storeCell, t.def);
				if (thing != null)
				{
					job.maxNumToCarry = t.def.stackLimit;
					if (fitInStoreCell)
					{
						job.maxNumToCarry -= thing.stackCount;
					}
				}
				else
				{
					job.maxNumToCarry = 99999;
				}
			}
			else
			{
				job.maxNumToCarry = 99999;
			}
			job.haulOpportunisticDuplicates = true;
			job.haulMode = HaulMode.ToCellStorage;
			return job;
		}
		public static StoragePriority StoragePriorityAtFor(IntVec3 c, Thing t)
		{
			if (!t.Spawned)
			{
				return StoragePriority.Unstored;
			}
			SlotGroup slotGroup = Find.SlotGroupManager.SlotGroupAt(c);
			if (slotGroup == null || !slotGroup.Settings.AllowedToAccept(t))
			{
				return StoragePriority.Unstored;
			}
			return slotGroup.Settings.Priority;
		}
		public static Job HaulAsideJobFor(Pawn p, Thing t)
		{
			if (!t.def.EverHaulable)
			{
				return null;
			}
			if (t.IsBurning())
			{
				return null;
			}
			if (!p.CanReserveAndReach(t, PathEndMode.ClosestTouch, p.NormalMaxDanger(), 1))
			{
				return null;
			}
			IntVec3 vec;
			if (!HaulAIUtility.TryFindSpotToPlaceHaulableCloseTo(t, p, t.PositionHeld, out vec))
			{
				return null;
			}
			return new Job(JobDefOf.HaulToCell, t, vec)
			{
				maxNumToCarry = 99999,
				haulOpportunisticDuplicates = false,
				haulMode = HaulMode.ToCellNonStorage,
				ignoreDesignations = true
			};
		}
		private static bool TryFindSpotToPlaceHaulableCloseTo(Thing haulable, Pawn worker, IntVec3 center, out IntVec3 spot)
		{
			for (int i = 1; i < GenRadial.NumCellsInRadius(7f); i++)
			{
				IntVec3 intVec = center + GenRadial.RadialPattern[i];
				if (HaulAIUtility.HaulablePlaceValidator(haulable, worker, intVec))
				{
					if (center.WithinRegions(intVec, 8, TraverseParms.For(worker, Danger.Deadly, TraverseMode.ByPawn, false)))
					{
						spot = intVec;
						return true;
					}
				}
			}
			for (int j = 1; j < GenRadial.RadialPattern.Length; j++)
			{
				IntVec3 intVec2 = center + GenRadial.RadialPattern[j];
				if (HaulAIUtility.HaulablePlaceValidator(haulable, worker, intVec2))
				{
					spot = intVec2;
					return true;
				}
			}
			spot = center;
			return false;
		}
		private static bool HaulablePlaceValidator(Thing haulable, Pawn worker, IntVec3 c)
		{
			if (!worker.CanReserveAndReach(c, PathEndMode.OnCell, worker.NormalMaxDanger(), 1))
			{
				return false;
			}
			if (GenPlace.HaulPlaceBlockerIn(haulable, c, true) != null)
			{
				return false;
			}
			if (!c.Standable())
			{
				return false;
			}
			if (haulable != null && haulable.def.BlockPlanting)
			{
				Zone zone = Find.ZoneManager.ZoneAt(c);
				if (zone is Zone_Growing)
				{
					return false;
				}
			}
			if (haulable.def.passability != Traversability.Standable)
			{
				for (int i = 0; i < 8; i++)
				{
					IntVec3 c2 = c + GenAdj.AdjacentCells[i];
					if (Find.DesignationManager.DesignationAt(c2, DesignationDefOf.Mine) != null)
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}

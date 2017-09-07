using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_SeekSafeTemperature : ThinkNode_JobGiver
	{
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (!pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious))
			{
				return null;
			}
			FloatRange tempRange = pawn.ComfortableTemperatureRange();
			if (tempRange.Includes(pawn.Position.GetTemperature()))
			{
				return new Job(JobDefOf.WaitSafeTemperature, 500, true);
			}
			Region region = JobGiver_SeekSafeTemperature.ClosestRegionWithinTemperatureRange(pawn.Position, tempRange, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false));
			if (region != null)
			{
				return new Job(JobDefOf.GotoSafeTemperature, region.RandomCell);
			}
			return null;
		}
		private static Region ClosestRegionWithinTemperatureRange(IntVec3 root, FloatRange tempRange, TraverseParms traverseParms)
		{
			RegionAndRoomUpdater.RebuildDirtyRegionsAndRooms();
			Region validRegionAt = Find.RegionGrid.GetValidRegionAt(root);
			if (validRegionAt == null)
			{
				return null;
			}
			RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParms, false);
			Region foundReg = null;
			RegionProcessor regionProcessor = delegate(Region r)
			{
				if (r.portal != null)
				{
					return false;
				}
				if (tempRange.Includes(r.Room.Temperature))
				{
					foundReg = r;
					return true;
				}
				return false;
			};
			RegionTraverser.BreadthFirstTraverse(validRegionAt, entryCondition, regionProcessor, 9999);
			return foundReg;
		}
	}
}

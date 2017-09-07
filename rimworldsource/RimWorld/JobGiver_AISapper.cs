using System;
using System.Linq;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public class JobGiver_AISapper : ThinkNode_JobGiver
	{
		private const float ReachDestDist = 10f;
		private const int CheckOverrideInterval = 500;
		private bool canMineNonMineables = true;
		public override ThinkNode DeepCopy()
		{
			JobGiver_AISapper jobGiver_AISapper = (JobGiver_AISapper)base.DeepCopy();
			jobGiver_AISapper.canMineNonMineables = this.canMineNonMineables;
			return jobGiver_AISapper;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			IntVec3 intVec = (IntVec3)pawn.mindState.duty.focus;
			if (intVec.IsValid && intVec.DistanceToSquared(pawn.Position) < 100f && intVec.GetRoom() == pawn.GetRoom() && intVec.WithinRegions(pawn.Position, 9, TraverseMode.NoPassClosedDoors))
			{
				pawn.GetLord().Notify_ReachedDutyLocation(pawn);
				return null;
			}
			if (!intVec.IsValid)
			{
				IAttackTarget attackTarget;
				if (!(
					from x in Find.AttackTargetsCache.GetPotentialTargetsFor(pawn)
					where !x.ThreatDisabled() && ((Thing)x).Faction == Faction.OfColony
					select x).TryRandomElement(out attackTarget))
				{
					return null;
				}
				intVec = ((Thing)attackTarget).Position;
			}
			TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassAnything, Danger.Deadly, false);
			traverseParms.pawn = pawn;
			traverseParms.canWalkDiagonally = false;
			PawnPath pawnPath = PathFinder.FindPath(pawn.Position, intVec, traverseParms, PathEndMode.OnCell);
			IntVec3 cellBeforeBlocker;
			Thing thing;
			try
			{
				thing = pawnPath.FirstBlockingBuilding(out cellBeforeBlocker);
			}
			finally
			{
				pawnPath.ReleaseToPool();
			}
			if (thing == null)
			{
				return new Job(JobDefOf.Goto, intVec, 500, true);
			}
			if (thing.def.mineable)
			{
				return JobGiver_AISapper.MineOrWaitJob(pawn, thing, cellBeforeBlocker);
			}
			if (pawn.equipment.Primary != null)
			{
				Verb primaryVerb = pawn.equipment.PrimaryEq.PrimaryVerb;
				if (primaryVerb.verbProps.ai_IsBuildingDestroyer && (!primaryVerb.verbProps.ai_IsIncendiary || thing.FlammableNow))
				{
					return new Job(JobDefOf.UseVerbOnThing)
					{
						targetA = thing,
						verbToUse = primaryVerb,
						expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange
					};
				}
			}
			if (this.canMineNonMineables)
			{
				return JobGiver_AISapper.MineOrWaitJob(pawn, thing, cellBeforeBlocker);
			}
			return JobGiver_AISapper.MeleeOrWaitJob(pawn, thing, cellBeforeBlocker);
		}
		private static Job MeleeOrWaitJob(Pawn pawn, Thing blocker, IntVec3 cellBeforeBlocker)
		{
			if (!pawn.CanReserve(blocker, 1))
			{
				return new Job(JobDefOf.Goto, CellFinder.RandomClosewalkCellNear(cellBeforeBlocker, 10), 500, true);
			}
			return new Job(JobDefOf.AttackMelee, blocker)
			{
				ignoreDesignations = true,
				expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
				checkOverrideOnExpire = true
			};
		}
		private static Job MineOrWaitJob(Pawn pawn, Thing blocker, IntVec3 cellBeforeBlocker)
		{
			if (!pawn.CanReserve(blocker, 1))
			{
				return new Job(JobDefOf.Goto, CellFinder.RandomClosewalkCellNear(cellBeforeBlocker, 10), 500, true);
			}
			return new Job(JobDefOf.Mine, blocker)
			{
				ignoreDesignations = true,
				expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
				checkOverrideOnExpire = true
			};
		}
	}
}

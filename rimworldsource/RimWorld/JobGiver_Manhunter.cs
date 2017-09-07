using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_Manhunter : ThinkNode_JobGiver
	{
		private const float WaitChance = 0.75f;
		private const int WaitTicks = 90;
		private const int MinMeleeChaseTicks = 420;
		private const int MaxMeleeChaseTicks = 900;
		private const int WanderOutsideDoorRegions = 9;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Thing thing = this.FindPawnTarget(pawn);
			if (thing != null && pawn.CanReach(thing, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
			{
				return this.MeleeAttackJob(pawn, thing);
			}
			Thing thing2 = this.FindTurretTarget(pawn);
			if (thing2 != null)
			{
				return this.MeleeAttackJob(pawn, thing2);
			}
			if (thing != null)
			{
				PawnPath pawnPath = PathFinder.FindPath(pawn.Position, thing.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell);
				IntVec3 randomCell;
				try
				{
					if (!pawnPath.Found)
					{
						Job result = null;
						return result;
					}
					IntVec3 loc;
					if (!pawnPath.TryFindLastCellBeforeBlockingDoor(out loc))
					{
						Log.Error(pawn + " did TryFindLastCellBeforeDoor but found none when it should have been one.");
						Job result = null;
						return result;
					}
					randomCell = CellFinder.RandomRegionNear(loc.GetRegion(), 9, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), null, null).RandomCell;
				}
				finally
				{
					pawnPath.ReleaseToPool();
				}
				return new Job(JobDefOf.Goto, randomCell);
			}
			return null;
		}
		private Job MeleeAttackJob(Pawn pawn, Thing target)
		{
			return new Job(JobDefOf.AttackMelee, target)
			{
				maxNumMeleeAttacks = 1,
				expiryInterval = Rand.Range(420, 900),
				checkOverrideOnExpire = true
			};
		}
		private Thing FindPawnTarget(Pawn pawn)
		{
			Predicate<Thing> predicate = delegate(Thing t)
			{
				if (t == pawn)
				{
					return false;
				}
				Pawn pawn2 = t as Pawn;
				return !pawn2.Downed && pawn2.RaceProps.intelligence >= Intelligence.ToolUser && pawn.HostileTo(pawn2);
			};
			Predicate<Thing> validator = predicate;
			return GenClosest.ClosestThingReachable(pawn.Position, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, true), 9999f, validator, null, -1, false);
		}
		private Thing FindTurretTarget(Pawn pawn)
		{
			return AttackTargetFinder.BestAttackTarget(pawn, (Thing t) => t is Building, 70f, 0f, TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, default(IntVec3), 3.40282347E+38f);
		}
	}
}

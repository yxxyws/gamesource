using System;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_HiveDefense : ThinkNode_JobGiver
	{
		private const float DistToHiveToStartAttacking = 10f;
		private const int MaxMeleeChaseTicks = 400;
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			Hive hive = pawn.mindState.hive;
			if (hive == null)
			{
				return null;
			}
			if (pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
			{
				return null;
			}
			if (pawn.TryGetAttackVerb(false) == null)
			{
				return null;
			}
			float maxTravelRadiusFromLocus = 32f;
			if (!hive.AngryNow)
			{
				maxTravelRadiusFromLocus = 10f;
			}
			Predicate<Thing> validator = delegate(Thing t)
			{
				if (!hive.AngryNow)
				{
					if (!GenSight.LineOfSight(hive.Position, t.Position, hive.OccupiedRect(), t.OccupiedRect()))
					{
						return false;
					}
				}
				else
				{
					if (HiveDefenseUtility.ShouldStopAttacking(pawn, t))
					{
						return false;
					}
				}
				return true;
			};
			Thing thing = AttackTargetFinder.BestAttackTarget(pawn, validator, 30f, 0f, TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedThreat, hive.Position, maxTravelRadiusFromLocus);
			if (thing == null)
			{
				return null;
			}
			return new Job(JobDefOf.AttackMelee, thing)
			{
				expiryInterval = 400,
				stopIfTooFarFromHive = true
			};
		}
	}
}

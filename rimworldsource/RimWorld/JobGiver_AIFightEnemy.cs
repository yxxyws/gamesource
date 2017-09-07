using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;
namespace RimWorld
{
	public abstract class JobGiver_AIFightEnemy : ThinkNode_JobGiver
	{
		private const int MinTargetDistanceToMove = 5;
		private const int TicksSinceEngageToLoseTarget = 400;
		private float targetAcquireRadius = 56f;
		private float targetKeepRadius = 65f;
		private bool needLOSToAcquireNonPawnTargets;
		public static readonly IntRange ExpiryInterval_ShooterSucceeded = new IntRange(450, 550);
		private static readonly IntRange ExpiryInterval_Melee = new IntRange(350, 450);
		protected abstract bool TryFindShootingPosition(Pawn pawn, out IntVec3 dest);
		protected virtual float GetFlagRadius(Pawn pawn)
		{
			return 999999f;
		}
		protected virtual IntVec3 GetFlagPosition(Pawn pawn)
		{
			return IntVec3.Invalid;
		}
		public override ThinkNode DeepCopy()
		{
			JobGiver_AIFightEnemy jobGiver_AIFightEnemy = (JobGiver_AIFightEnemy)base.DeepCopy();
			jobGiver_AIFightEnemy.targetAcquireRadius = this.targetAcquireRadius;
			jobGiver_AIFightEnemy.targetKeepRadius = this.targetKeepRadius;
			jobGiver_AIFightEnemy.needLOSToAcquireNonPawnTargets = this.needLOSToAcquireNonPawnTargets;
			return jobGiver_AIFightEnemy;
		}
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			this.UpdateEnemyTarget(pawn);
			Thing enemyTarget = pawn.mindState.enemyTarget;
			if (enemyTarget == null)
			{
				return null;
			}
			bool allowManualCastWeapons = !pawn.IsColonist;
			Verb verb = pawn.TryGetAttackVerb(allowManualCastWeapons);
			if (verb == null)
			{
				return null;
			}
			if (verb.verbProps.MeleeRange)
			{
				return new Job(JobDefOf.AttackMelee, enemyTarget)
				{
					expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_Melee.RandomInRange,
					checkOverrideOnExpire = true
				};
			}
			bool flag = CoverUtility.CalculateOverallBlockChance(pawn.Position, enemyTarget.Position) > 0.01f;
			bool flag2 = pawn.Position.Standable();
			bool flag3 = verb.CanHitTarget(enemyTarget);
			bool flag4 = (pawn.Position - enemyTarget.Position).LengthHorizontalSquared < 25f;
			if ((flag && flag2 && flag3) || (flag4 && flag3))
			{
				return new Job(JobDefOf.WaitCombat, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
			}
			IntVec3 intVec;
			if (!this.TryFindShootingPosition(pawn, out intVec))
			{
				return null;
			}
			if (intVec == pawn.Position)
			{
				return new Job(JobDefOf.WaitCombat, JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange, true);
			}
			Find.PawnDestinationManager.ReserveDestinationFor(pawn, intVec);
			return new Job(JobDefOf.Goto, intVec)
			{
				expiryInterval = JobGiver_AIFightEnemy.ExpiryInterval_ShooterSucceeded.RandomInRange,
				checkOverrideOnExpire = true
			};
		}
		protected virtual void UpdateEnemyTarget(Pawn pawn)
		{
			Thing thing = pawn.mindState.enemyTarget;
			if (thing != null && (thing.Destroyed || Find.TickManager.TicksGame - pawn.mindState.lastEngageTargetTick > 400 || !pawn.CanReach(thing, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn) || (pawn.Position - thing.Position).LengthHorizontalSquared > this.targetKeepRadius * this.targetKeepRadius || ((IAttackTarget)thing).ThreatDisabled()))
			{
				thing = null;
			}
			if (thing == null)
			{
				thing = this.FindAttackTarget(pawn);
				if (thing != null)
				{
					pawn.mindState.Notify_EngagedTarget();
					Lord lord = Find.LordManager.LordOf(pawn);
					if (lord != null)
					{
						lord.Notify_PawnAcquiredTarget(pawn, thing);
					}
				}
			}
			else
			{
				Thing thing2 = this.FindAttackTarget(pawn);
				if (thing2 != null && thing2 != thing)
				{
					pawn.mindState.Notify_EngagedTarget();
					thing = thing2;
				}
			}
			pawn.mindState.enemyTarget = thing;
		}
		private Thing FindAttackTarget(Pawn pawn)
		{
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat;
			if (this.needLOSToAcquireNonPawnTargets)
			{
				targetScanFlags |= TargetScanFlags.NeedLOSToNonPawns;
			}
			if (this.PrimaryVerbIsIncendiary(pawn))
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return AttackTargetFinder.BestAttackTarget(pawn, null, this.targetAcquireRadius, 0f, targetScanFlags, this.GetFlagPosition(pawn), this.GetFlagRadius(pawn));
		}
		private bool PrimaryVerbIsIncendiary(Pawn pawn)
		{
			if (pawn.equipment != null && pawn.equipment.Primary != null)
			{
				List<VerbProperties> verbs = pawn.equipment.Primary.def.Verbs;
				for (int i = 0; i < verbs.Count; i++)
				{
					if (verbs[i].isPrimary)
					{
						return verbs[i].ai_IsIncendiary;
					}
				}
			}
			return false;
		}
	}
}

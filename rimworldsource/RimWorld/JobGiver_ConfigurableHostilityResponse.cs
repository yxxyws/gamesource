using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobGiver_ConfigurableHostilityResponse : ThinkNode_JobGiver
	{
		private static List<Thing> tmpThreats = new List<Thing>();
		protected override Job TryGiveTerminalJob(Pawn pawn)
		{
			if (!pawn.IsColonist)
			{
				return null;
			}
			if (PawnUtility.PlayerForcedJobNowOrSoon(pawn))
			{
				return null;
			}
			switch (pawn.playerSettings.hostilityResponse)
			{
			case HostilityResponseMode.Ignore:
				return null;
			case HostilityResponseMode.Attack:
				return this.TryGetAttackNearbyEnemyJob(pawn);
			case HostilityResponseMode.Flee:
				return this.TryGetFleeJob(pawn);
			default:
				return null;
			}
		}
		private Job TryGetAttackNearbyEnemyJob(Pawn pawn)
		{
			if (pawn.story != null && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
			{
				return null;
			}
			bool flag = pawn.equipment.Primary == null || pawn.equipment.Primary.def.IsMeleeWeapon;
			float maxTargDist = 8f;
			if (!flag)
			{
				maxTargDist = Mathf.Clamp(pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.range * 0.66f, 2f, 20f);
			}
			Thing thing = AttackTargetFinder.BestAttackTarget(pawn, null, maxTargDist, 0f, TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat, default(IntVec3), 3.40282347E+38f);
			if (thing == null)
			{
				return null;
			}
			if (flag)
			{
				return new Job(JobDefOf.AttackMelee, thing);
			}
			return new Job(JobDefOf.AttackStatic, thing);
		}
		private Job TryGetFleeJob(Pawn pawn)
		{
			if (!SelfDefenseUtility.ShouldStartFleeing(pawn))
			{
				return null;
			}
			JobGiver_ConfigurableHostilityResponse.tmpThreats.Clear();
			List<IAttackTarget> potentialTargetsFor = Find.AttackTargetsCache.GetPotentialTargetsFor(pawn);
			for (int i = 0; i < potentialTargetsFor.Count; i++)
			{
				IAttackTarget attackTarget = potentialTargetsFor[i];
				if (!attackTarget.ThreatDisabled())
				{
					JobGiver_ConfigurableHostilityResponse.tmpThreats.Add((Thing)attackTarget);
				}
			}
			if (!JobGiver_ConfigurableHostilityResponse.tmpThreats.Any<Thing>())
			{
				Log.Warning(pawn.LabelBaseShort + " decided to flee but there is no any threat around.");
				return null;
			}
			IntVec3 fleeDest = this.GetFleeDest(pawn, JobGiver_ConfigurableHostilityResponse.tmpThreats);
			JobGiver_ConfigurableHostilityResponse.tmpThreats.Clear();
			return new Job(JobDefOf.FleeAndCower, fleeDest);
		}
		private IntVec3 GetFleeDest(Pawn pawn, List<Thing> threats)
		{
			IntVec3 bestPos = pawn.Position;
			float bestScore = -1f;
			TraverseParms traverseParms = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
			RegionTraverser.BreadthFirstTraverse(pawn.Position.GetRegion(), (Region from, Region reg) => reg.Allows(traverseParms, false), delegate(Region reg)
			{
				Danger danger = reg.DangerFor(pawn);
				foreach (IntVec3 current in reg.Cells)
				{
					if (current.Standable())
					{
						if (reg.portal == null)
						{
							Thing thing = null;
							float num = 0f;
							for (int i = 0; i < threats.Count; i++)
							{
								float num2 = current.DistanceToSquared(threats[i].Position);
								if (thing == null || num2 < num)
								{
									thing = threats[i];
									num = num2;
								}
							}
							float num3 = Mathf.Sqrt(num);
							float f = Mathf.Min(num3, 23f);
							float num4 = Mathf.Pow(f, 1.2f);
							num4 *= Mathf.InverseLerp(50f, 0f, (current - pawn.Position).LengthHorizontal);
							if (current.GetRoom() != thing.GetRoom())
							{
								num4 *= 4.2f;
							}
							else
							{
								if (num3 < 8f)
								{
									num4 *= 0.05f;
								}
							}
							if (Find.PawnDestinationManager.DestinationIsReserved(current, pawn))
							{
								num4 *= 0.5f;
							}
							if (danger == Danger.Deadly)
							{
								num4 *= 0.8f;
							}
							if (num4 > bestScore)
							{
								bestPos = current;
								bestScore = num4;
							}
						}
					}
				}
				return false;
			}, 20);
			return bestPos;
		}
	}
}

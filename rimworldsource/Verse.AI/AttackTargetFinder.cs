using RimWorld;
using System;
using System.Collections.Generic;
namespace Verse.AI
{
	public static class AttackTargetFinder
	{
		private static List<Thing> tmpTargets = new List<Thing>();
		private static List<IntVec3> tempDestList = new List<IntVec3>();
		private static List<IntVec3> tempSourceList = new List<IntVec3>();
		public static Thing BestAttackTarget(Thing searcher, Predicate<Thing> validator, float maxTargDist, float minTargDist, TargetScanFlags flags, IntVec3 locus = default(IntVec3), float maxTravelRadiusFromLocus = 3.40282347E+38f)
		{
			Pawn searcherPawn = searcher as Pawn;
			Verb attackVerb = AttackTargetFinder.GetAttackVerb(searcher);
			if (attackVerb == null)
			{
				Log.Error("BestAttackTarget with " + searcher + " who has no attack verb.");
				return null;
			}
			bool onlyTargetMachines = attackVerb != null && attackVerb.verbProps.projectileDef != null && attackVerb.verbProps.projectileDef.projectile.damageDef == DamageDefOf.EMP;
			float minDistanceSquared = minTargDist * minTargDist;
			float num = maxTravelRadiusFromLocus + attackVerb.verbProps.range;
			float maxLocusDistSquared = num * num;
			Predicate<Thing> predicate = delegate(Thing t)
			{
				if (t == searcher)
				{
					return false;
				}
				if (minDistanceSquared > 0f && (searcher.Position - t.Position).LengthHorizontalSquared < minDistanceSquared)
				{
					return false;
				}
				if (maxTravelRadiusFromLocus < 9999f && (t.Position - locus).LengthHorizontalSquared > maxLocusDistSquared)
				{
					return false;
				}
				if (!searcher.HostileTo(t))
				{
					return false;
				}
				if (validator != null && !validator(t))
				{
					return false;
				}
				if ((byte)(flags & TargetScanFlags.NeedLOSToAll) != 0 && !searcher.CanSee(t))
				{
					if (t is Pawn)
					{
						if ((byte)(flags & TargetScanFlags.NeedLOSToPawns) != 0)
						{
							return false;
						}
					}
					else
					{
						if ((byte)(flags & TargetScanFlags.NeedLOSToNonPawns) != 0)
						{
							return false;
						}
					}
				}
				if ((byte)(flags & TargetScanFlags.NeedThreat) != 0 && ((IAttackTarget)t).ThreatDisabled())
				{
					return false;
				}
				Pawn pawn = t as Pawn;
				if (onlyTargetMachines && pawn != null && pawn.RaceProps.IsFlesh)
				{
					return false;
				}
				if ((byte)(flags & TargetScanFlags.NeedNonBurning) != 0 && t.IsBurning())
				{
					return false;
				}
				if (searcher.def.race != null && searcher.def.race.intelligence >= Intelligence.Humanlike)
				{
					CompExplosive compExplosive = t.TryGetComp<CompExplosive>();
					if (compExplosive != null && compExplosive.wickStarted)
					{
						return false;
					}
				}
				return true;
			};
			if (AttackTargetFinder.HasRangedAttack(searcher))
			{
				AttackTargetFinder.tmpTargets.Clear();
				List<IAttackTarget> potentialTargetsFor = Find.AttackTargetsCache.GetPotentialTargetsFor(searcher);
				for (int i = 0; i < potentialTargetsFor.Count; i++)
				{
					AttackTargetFinder.tmpTargets.Add((Thing)potentialTargetsFor[i]);
				}
				if ((byte)(flags & TargetScanFlags.NeedReachable) != 0)
				{
					Predicate<Thing> oldValidator = predicate;
					predicate = delegate(Thing t)
					{
						if (!oldValidator(t))
						{
							return false;
						}
						if (searcherPawn != null)
						{
							if (!searcherPawn.CanReach(t, PathEndMode.Touch, Danger.Some, false, TraverseMode.ByPawn))
							{
								return false;
							}
						}
						else
						{
							if (!searcher.Position.CanReach(t, PathEndMode.Touch, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false)))
							{
								return false;
							}
						}
						return true;
					};
				}
				Thing result = GenClosest.ClosestThing_Global(searcher.Position, AttackTargetFinder.tmpTargets, maxTargDist, predicate);
				AttackTargetFinder.tmpTargets.Clear();
				return result;
			}
			if (searcherPawn != null && searcherPawn.mindState.duty != null && searcherPawn.mindState.duty.radius > 0f)
			{
				Predicate<Thing> oldValidator = predicate;
				predicate = ((Thing t) => oldValidator(t) && t.Position.InHorDistOf(searcherPawn.mindState.duty.focus.Cell, searcherPawn.mindState.duty.radius));
			}
			int searchRegionsMax = (maxTargDist <= 800f) ? 40 : -1;
			return GenClosest.ClosestThingReachable(searcher.Position, ThingRequest.ForGroup(ThingRequestGroup.AttackTarget), PathEndMode.Touch, TraverseParms.For(searcherPawn, Danger.Deadly, TraverseMode.ByPawn, false), maxTargDist, predicate, null, searchRegionsMax, false);
		}
		private static Verb GetAttackVerb(Thing attacker)
		{
			Pawn pawn = attacker as Pawn;
			if (pawn != null)
			{
				return pawn.TryGetAttackVerb(!pawn.IsColonist);
			}
			Building_Turret building_Turret = attacker as Building_Turret;
			if (building_Turret != null)
			{
				return building_Turret.AttackVerb;
			}
			return null;
		}
		private static bool HasRangedAttack(Thing t)
		{
			if (t is Building_Turret)
			{
				return true;
			}
			Pawn pawn = t as Pawn;
			if (pawn == null)
			{
				Log.ErrorOnce("Unknown target searcher: " + t, 7547344);
				return true;
			}
			Thing thing = pawn.MannedThing();
			if (thing != null && thing is Building_Turret)
			{
				return true;
			}
			Verb verb = pawn.TryGetAttackVerb(true);
			return verb != null && !verb.verbProps.MeleeRange;
		}
		public static Thing BestShootTargetFromCurrentPosition(Thing searcher, Predicate<Thing> validator, float maxDistance, float minDistance, TargetScanFlags flags)
		{
			return AttackTargetFinder.BestAttackTarget(searcher, validator, maxDistance, minDistance, flags, default(IntVec3), 3.40282347E+38f);
		}
		public static bool CanSee(this Thing seer, Thing target)
		{
			ShootLeanUtility.CalcShootableCellsOf(AttackTargetFinder.tempDestList, target);
			for (int i = 0; i < AttackTargetFinder.tempDestList.Count; i++)
			{
				if (GenSight.LineOfSight(seer.Position, AttackTargetFinder.tempDestList[i], true))
				{
					return true;
				}
			}
			ShootLeanUtility.LeanShootingSourcesFromTo(seer.Position, target.Position, AttackTargetFinder.tempSourceList);
			for (int j = 0; j < AttackTargetFinder.tempSourceList.Count; j++)
			{
				for (int k = 0; k < AttackTargetFinder.tempDestList.Count; k++)
				{
					if (GenSight.LineOfSight(AttackTargetFinder.tempSourceList[j], AttackTargetFinder.tempDestList[k], true))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}

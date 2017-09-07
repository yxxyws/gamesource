using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Verse
{
	public class Verb_LaunchProjectile : Verb
	{
		protected override bool TryCastShot()
		{
			ShootLine shootLine;
			bool flag = base.TryFindShootLineFromTo(this.caster.Position, this.currentTarget, out shootLine);
			if (this.verbProps.stopBurstWithoutLos && !flag)
			{
				return false;
			}
			Vector3 drawPos = this.caster.DrawPos;
			Projectile projectile = (Projectile)ThingMaker.MakeThing(this.verbProps.projectileDef, null);
			GenSpawn.Spawn(projectile, shootLine.Source);
			float num = this.verbProps.forcedMissRadius;
			float lengthHorizontalSquared = (this.currentTarget.Cell - this.caster.Position).LengthHorizontalSquared;
			if (lengthHorizontalSquared < 9f)
			{
				num = 0f;
			}
			else
			{
				if (lengthHorizontalSquared < 25f)
				{
					num *= 0.5f;
				}
				else
				{
					if (lengthHorizontalSquared < 49f)
					{
						num *= 0.8f;
					}
				}
			}
			if (num > 0.5f)
			{
				int max = GenRadial.NumCellsInRadius(this.verbProps.forcedMissRadius);
				int num2 = Rand.Range(0, max);
				if (num2 > 0)
				{
					IntVec3 vec = this.currentTarget.Cell + GenRadial.RadialPattern[num2];
					projectile.canFreeIntercept = !this.CanHitTargetFrom(this.caster.Position, vec);
					projectile.Launch(this.caster, drawPos, vec, this.ownerEquipment);
					if (this.currentTarget.HasThing)
					{
						projectile.AssignedMissTarget = this.currentTarget.Thing;
					}
					return true;
				}
			}
			HitReport hitReport = this.HitReportFor(this.currentTarget);
			if (Rand.Value > hitReport.TotalNonWildShotChance)
			{
				shootLine.ChangeDestToMissWild();
				projectile.canFreeIntercept = !this.CanHitTargetFrom(this.caster.Position, shootLine.Dest);
				projectile.Launch(this.caster, drawPos, shootLine.Dest, this.ownerEquipment);
				return true;
			}
			if (Rand.Value > hitReport.HitChanceThroughCover && this.currentTarget.Thing != null && this.currentTarget.Thing.def.category == ThingCategory.Pawn)
			{
				Thing thing = hitReport.covers.RandomElementByWeight((CoverInfo c) => c.BlockChance).Thing;
				projectile.canFreeIntercept = true;
				projectile.Launch(this.caster, drawPos, thing, this.ownerEquipment);
				return true;
			}
			if (this.currentTarget.Thing != null)
			{
				projectile.Launch(this.caster, drawPos, this.currentTarget.Thing, this.ownerEquipment);
			}
			else
			{
				projectile.Launch(this.caster, drawPos, shootLine.Dest, this.ownerEquipment);
			}
			return true;
		}
		public HitReport HitReportFor(TargetInfo target)
		{
			IntVec3 cell = target.Cell;
			HitReport hitReport = new HitReport();
			hitReport.shotDistance = (cell - this.caster.Position).LengthHorizontal;
			hitReport.target = target;
			if (!this.verbProps.canMiss)
			{
				hitReport.hitChanceThroughPawnStat = 1f;
				hitReport.covers = new List<CoverInfo>();
				hitReport.coversOverallBlockChance = 0f;
			}
			else
			{
				float f = 1f;
				if (base.CasterIsPawn)
				{
					f = base.CasterPawn.GetStatValue(StatDefOf.ShootingAccuracy, true);
				}
				hitReport.hitChanceThroughPawnStat = Mathf.Pow(f, hitReport.shotDistance);
				if (hitReport.hitChanceThroughPawnStat < 0.0201f)
				{
					hitReport.hitChanceThroughPawnStat = 0.0201f;
				}
				if (base.CasterIsPawn)
				{
					hitReport.hitChanceThroughSightEfficiency = base.CasterPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Sight);
				}
				hitReport.hitChanceThroughEquipment = this.verbProps.HitMultiplierAtDist(hitReport.shotDistance, this.ownerEquipment);
				hitReport.forcedMissRadius = this.verbProps.forcedMissRadius;
				hitReport.covers = CoverUtility.CalculateCoverGiverSet(cell, this.caster.Position);
				hitReport.coversOverallBlockChance = CoverUtility.CalculateOverallBlockChance(cell, this.caster.Position);
				hitReport.targetLighting = Find.GlowGrid.PsychGlowAt(cell);
				if (!this.caster.Position.Roofed() && !target.Cell.Roofed())
				{
					hitReport.hitChanceThroughWeather = Find.WeatherManager.CurWeatherAccuracyMultiplier;
				}
				if (target.HasThing)
				{
					Pawn pawn = target.Thing as Pawn;
					if (pawn != null)
					{
						float num = pawn.BodySize;
						num = Mathf.Clamp(num, 0.5f, 2f);
						hitReport.hitChanceThroughTargetSize = num;
					}
				}
			}
			return hitReport;
		}
		public override float HighlightFieldRadiusAroundTarget()
		{
			return this.verbProps.projectileDef.projectile.explosionRadius;
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class CompExplosive : ThingComp
	{
		public bool wickStarted;
		protected int wickTicksLeft;
		protected Sustainer wickSoundSustainer;
		private static readonly int PawnNotifyCellCount = GenRadial.NumCellsInRadius(4.5f);
		private bool detonated;
		protected int StartWickThreshold
		{
			get
			{
				return Mathf.RoundToInt(this.Props.startWickHitPointsPercent * (float)this.parent.MaxHitPoints);
			}
		}
		public CompProperties_Explosive Props
		{
			get
			{
				return (CompProperties_Explosive)this.props;
			}
		}
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.LookValue<bool>(ref this.wickStarted, "wickStarted", false, false);
			Scribe_Values.LookValue<int>(ref this.wickTicksLeft, "wickTicksLeft", 0, false);
		}
		public override void CompTick()
		{
			if (this.wickStarted)
			{
				if (this.wickSoundSustainer == null)
				{
					this.StartWickSustainer();
				}
				else
				{
					this.wickSoundSustainer.Maintain();
				}
				this.wickTicksLeft--;
				if (this.wickTicksLeft <= 0)
				{
					this.Detonate();
				}
			}
		}
		private void StartWickSustainer()
		{
			SoundDefOf.MetalHitImportant.PlayOneShot(this.parent.Position);
			SoundInfo info = SoundInfo.InWorld(this.parent, MaintenanceType.PerTick);
			this.wickSoundSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
		}
		public override void PostDraw()
		{
			if (this.wickStarted)
			{
				OverlayDrawer.DrawOverlay(this.parent, OverlayTypes.BurningWick);
			}
		}
		public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			if (this.Props.startWickOnDamageTaken != null && dinfo.Def == this.Props.startWickOnDamageTaken)
			{
				this.StartWick();
				absorbed = true;
			}
			else
			{
				absorbed = false;
			}
		}
		public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
			if (this.parent.HitPoints <= 0 && this.StartWickThreshold >= 0)
			{
				if (dinfo.Def.externalViolence)
				{
					this.Detonate();
				}
			}
			else
			{
				if (this.wickStarted && dinfo.Def == DamageDefOf.Stun)
				{
					this.StopWick();
				}
				else
				{
					if (!this.wickStarted && this.parent.HitPoints <= this.StartWickThreshold && dinfo.Def.externalViolence)
					{
						this.StartWick();
					}
				}
			}
		}
		public void StartWick()
		{
			if (this.wickStarted)
			{
				return;
			}
			this.wickStarted = true;
			this.wickTicksLeft = this.Props.wickTicks.RandomInRange;
			this.StartWickSustainer();
			if (this.Props.explosiveDamageType.externalViolence)
			{
				Room room = this.parent.GetRoom();
				for (int i = 0; i < CompExplosive.PawnNotifyCellCount; i++)
				{
					IntVec3 c = this.parent.Position + GenRadial.RadialPattern[i];
					if (c.InBounds())
					{
						List<Thing> thingList = c.GetThingList();
						for (int j = 0; j < thingList.Count; j++)
						{
							Pawn pawn = thingList[j] as Pawn;
							if (pawn != null && pawn.RaceProps.intelligence >= Intelligence.Humanlike && pawn.Position.GetRoom() == room && GenSight.LineOfSight(this.parent.Position, pawn.Position, true))
							{
								pawn.mindState.Notify_DangerousWickStarted(this.parent);
							}
						}
					}
				}
			}
		}
		public void StopWick()
		{
			this.wickStarted = false;
		}
		protected void Detonate()
		{
			if (this.detonated)
			{
				return;
			}
			this.detonated = true;
			if (!this.parent.Destroyed)
			{
				this.parent.Destroy(DestroyMode.Kill);
			}
			CompProperties_Explosive props = this.Props;
			float num = props.explosiveRadius;
			if (this.parent.stackCount > 1 && props.explosiveExpandPerStackcount > 0f)
			{
				num += Mathf.Sqrt((float)(this.parent.stackCount - 1) * props.explosiveExpandPerStackcount);
			}
			if (props.explosionEffect != null)
			{
				Effecter effecter = props.explosionEffect.Spawn();
				effecter.Trigger(this.parent.Position, this.parent.Position);
				effecter.Cleanup();
			}
			ThingDef postExplosionSpawnThingDef = props.postExplosionSpawnThingDef;
			float postExplosionSpawnChance = props.postExplosionSpawnChance;
			bool applyDamageToExplosionCellsNeighbors = props.applyDamageToExplosionCellsNeighbors;
			GenExplosion.DoExplosion(this.parent.Position, num, props.explosiveDamageType, this.parent, null, null, null, postExplosionSpawnThingDef, postExplosionSpawnChance, applyDamageToExplosionCellsNeighbors, props.preExplosionSpawnThingDef, props.preExplosionSpawnChance);
		}
	}
}

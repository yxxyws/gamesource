using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public abstract class Projectile : ThingWithComps
	{
		private const float MinFreeInterceptDistance = 4f;
		protected Vector3 origin;
		protected Vector3 destination;
		protected Thing assignedTarget;
		public bool canFreeIntercept;
		protected ThingDef equipmentDef;
		protected Thing launcher;
		private Thing assignedMissTargetInt;
		protected bool landed;
		protected int ticksToImpact;
		private Sustainer ambientSustainer;
		private static List<IntVec3> checkedCells = new List<IntVec3>();
		public Thing AssignedMissTarget
		{
			get
			{
				return this.assignedMissTargetInt;
			}
			set
			{
				if (value.def.Fillage == FillCategory.Full)
				{
					return;
				}
				this.assignedMissTargetInt = value;
			}
		}
		protected int StartingTicksToImpact
		{
			get
			{
				int num = Mathf.RoundToInt((this.origin - this.destination).magnitude / (this.def.projectile.speed / 100f));
				if (num < 1)
				{
					num = 1;
				}
				return num;
			}
		}
		protected IntVec3 DestinationCell
		{
			get
			{
				return new IntVec3(this.destination);
			}
		}
		public virtual Vector3 ExactPosition
		{
			get
			{
				Vector3 b = (this.destination - this.origin) * (1f - (float)this.ticksToImpact / (float)this.StartingTicksToImpact);
				return this.origin + b + Vector3.up * this.def.Altitude;
			}
		}
		public virtual Quaternion ExactRotation
		{
			get
			{
				return Quaternion.LookRotation(this.destination - this.origin);
			}
		}
		public override Vector3 DrawPos
		{
			get
			{
				return this.ExactPosition;
			}
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<Vector3>(ref this.origin, "origin", default(Vector3), false);
			Scribe_Values.LookValue<Vector3>(ref this.destination, "destination", default(Vector3), false);
			Scribe_References.LookReference<Thing>(ref this.assignedTarget, "assignedTarget", false);
			Scribe_Values.LookValue<bool>(ref this.canFreeIntercept, "canFreeIntercept", false, false);
			Scribe_Defs.LookDef<ThingDef>(ref this.equipmentDef, "equipmentDef");
			Scribe_References.LookReference<Thing>(ref this.launcher, "launcher", false);
			Scribe_References.LookReference<Thing>(ref this.assignedMissTargetInt, "assignedMissTarget", false);
			Scribe_Values.LookValue<bool>(ref this.landed, "landed", false, false);
			Scribe_Values.LookValue<int>(ref this.ticksToImpact, "ticksToImpact", 0, false);
		}
		public void Launch(Thing launcher, TargetInfo targ, Thing equipment = null)
		{
			this.Launch(launcher, base.Position.ToVector3Shifted(), targ, null);
		}
		public void Launch(Thing launcher, Vector3 origin, TargetInfo targ, Thing equipment = null)
		{
			this.launcher = launcher;
			this.origin = origin;
			if (equipment != null)
			{
				this.equipmentDef = equipment.def;
			}
			else
			{
				this.equipmentDef = null;
			}
			if (targ.Thing != null)
			{
				this.assignedTarget = targ.Thing;
			}
			this.destination = targ.Cell.ToVector3Shifted() + new Vector3(Rand.Range(-0.3f, 0.3f), 0f, Rand.Range(-0.3f, 0.3f));
			this.ticksToImpact = this.StartingTicksToImpact;
			if (!this.def.projectile.soundAmbient.NullOrUndefined())
			{
				SoundInfo info = SoundInfo.InWorld(this, MaintenanceType.PerTick);
				this.ambientSustainer = this.def.projectile.soundAmbient.TrySpawnSustainer(info);
			}
		}
		public override void Tick()
		{
			base.Tick();
			if (this.landed)
			{
				return;
			}
			Vector3 exactPosition = this.ExactPosition;
			this.ticksToImpact--;
			if (!this.ExactPosition.InBounds())
			{
				this.ticksToImpact++;
				base.Position = this.ExactPosition.ToIntVec3();
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			Vector3 exactPosition2 = this.ExactPosition;
			if ((this.def.projectile.alwaysFreeIntercept || (!this.def.projectile.flyOverhead && this.canFreeIntercept)) && this.CheckForFreeInterceptBetween(exactPosition, exactPosition2))
			{
				return;
			}
			base.Position = this.ExactPosition.ToIntVec3();
			if (this.ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && this.def.projectile.soundImpactAnticipate != null)
			{
				this.def.projectile.soundImpactAnticipate.PlayOneShot(this);
			}
			if (this.ticksToImpact <= 0)
			{
				if (this.DestinationCell.InBounds())
				{
					base.Position = this.DestinationCell;
				}
				this.ImpactSomething();
				return;
			}
			if (this.ambientSustainer != null)
			{
				this.ambientSustainer.Maintain();
			}
		}
		private bool CheckForFreeInterceptBetween(Vector3 lastExactPos, Vector3 newExactPos)
		{
			IntVec3 intVec = lastExactPos.ToIntVec3();
			IntVec3 intVec2 = newExactPos.ToIntVec3();
			if (intVec2 == intVec)
			{
				return false;
			}
			if (!intVec.InBounds() || !intVec2.InBounds())
			{
				return false;
			}
			if ((intVec2 - intVec).LengthManhattan == 1)
			{
				return this.CheckForFreeIntercept(intVec2);
			}
			if (this.origin.ToIntVec3().DistanceToSquared(intVec2) > 16f)
			{
				Vector3 vector = lastExactPos;
				Vector3 v = newExactPos - lastExactPos;
				Vector3 b = v.normalized * 0.2f;
				int num = (int)(v.MagnitudeHorizontal() / 0.2f);
				Projectile.checkedCells.Clear();
				int num2 = 0;
				while (true)
				{
					vector += b;
					IntVec3 intVec3 = vector.ToIntVec3();
					if (!Projectile.checkedCells.Contains(intVec3))
					{
						if (this.CheckForFreeIntercept(intVec3))
						{
							break;
						}
						Projectile.checkedCells.Add(intVec3);
					}
					num2++;
					if (num2 > num)
					{
						return false;
					}
					if (intVec3 == intVec2)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		private bool CheckForFreeIntercept(IntVec3 c)
		{
			float num = (c.ToVector3Shifted() - this.origin).MagnitudeHorizontalSquared();
			if (num < 16f)
			{
				return false;
			}
			List<Thing> list = Find.ThingGrid.ThingsListAt(c);
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (thing != this.AssignedMissTarget)
				{
					if (thing != this.launcher)
					{
						if (thing.def.Fillage == FillCategory.Full)
						{
							this.Impact(thing);
							return true;
						}
						if (thing.def.category == ThingCategory.Pawn)
						{
							Pawn pawn = (Pawn)thing;
							float num2 = 0.45f;
							if (pawn.GetPosture() != PawnPosture.Standing)
							{
								num2 *= 0.1f;
							}
							if (num < 49f)
							{
								num2 *= 0.5f;
							}
							else
							{
								if (num < 100f)
								{
									num2 *= 0.75f;
								}
							}
							num2 *= pawn.BodySize;
							if (Rand.Value < num2)
							{
								this.Impact(pawn);
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		public override void Draw()
		{
			Graphics.DrawMesh(MeshPool.plane10, this.DrawPos, this.ExactRotation, this.def.DrawMatSingle, 0);
			base.Comps_PostDraw();
		}
		private void ImpactSomething()
		{
			if (this.def.projectile.flyOverhead)
			{
				RoofDef roofDef = Find.RoofGrid.RoofAt(base.Position);
				if (roofDef != null && roofDef.isThickRoof)
				{
					this.def.projectile.soundHitThickRoof.PlayOneShot(base.Position);
					this.Destroy(DestroyMode.Vanish);
					return;
				}
			}
			if (this.assignedTarget != null)
			{
				Pawn pawn = this.assignedTarget as Pawn;
				if (pawn != null && pawn.Downed && (this.origin - this.destination).MagnitudeHorizontalSquared() > 5f && Rand.Value < 0.2f)
				{
					this.Impact(null);
					return;
				}
				this.Impact(this.assignedTarget);
				return;
			}
			else
			{
				Thing thing = Find.ThingGrid.ThingAt(base.Position, ThingCategory.Pawn);
				if (thing != null)
				{
					this.Impact(thing);
					return;
				}
				List<Thing> list = Find.ThingGrid.ThingsListAt(base.Position);
				for (int i = 0; i < list.Count; i++)
				{
					Thing thing2 = list[i];
					if (thing2.def.fillPercent > 0f || thing2.def.passability != Traversability.Standable)
					{
						this.Impact(thing2);
						return;
					}
				}
				this.Impact(null);
				return;
			}
		}
		protected virtual void Impact(Thing hitThing)
		{
			this.Destroy(DestroyMode.Vanish);
		}
		public void ForceInstantImpact()
		{
			if (!this.DestinationCell.InBounds())
			{
				this.Destroy(DestroyMode.Vanish);
				return;
			}
			this.ticksToImpact = 0;
			base.Position = this.DestinationCell;
			this.ImpactSomething();
		}
	}
}

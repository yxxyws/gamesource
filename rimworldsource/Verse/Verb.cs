using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.Sound;
namespace Verse
{
	public abstract class Verb : ILoadReferenceable, IExposable
	{
		public VerbProperties verbProps;
		public Thing caster;
		public ThingWithComps ownerEquipment;
		public HediffComp_VerbGiver ownerHediffComp;
		public int loadID = -1;
		public VerbState state;
		protected TargetInfo currentTarget = null;
		protected int burstShotsLeft;
		protected int ticksToNextBurstShot;
		protected bool isSurpriseAttack;
		public Action castCompleteCallback;
		private static List<IntVec3> tempDestList = new List<IntVec3>();
		private static List<IntVec3> tempLeanShootSources = new List<IntVec3>();
		public Pawn CasterPawn
		{
			get
			{
				return this.caster as Pawn;
			}
		}
		public bool CasterIsPawn
		{
			get
			{
				return this.caster is Pawn;
			}
		}
		protected virtual int ShotsPerBurst
		{
			get
			{
				return 1;
			}
		}
		public virtual Texture2D UIIcon
		{
			get
			{
				if (this.ownerEquipment != null)
				{
					return this.ownerEquipment.def.uiIcon;
				}
				return BaseContent.BadTex;
			}
		}
		public bool Bursting
		{
			get
			{
				return this.burstShotsLeft > 0;
			}
		}
		public float GetDamageFactorFor(Pawn pawn)
		{
			if (pawn != null)
			{
				if (this.ownerHediffComp != null)
				{
					return PawnCapacityUtility.CalculatePartEfficiency(this.ownerHediffComp.Pawn.health.hediffSet, this.ownerHediffComp.parent.Part, true);
				}
				if (this.verbProps.linkedBodyPartsGroup != null)
				{
					return PawnCapacityUtility.CalculateNaturalPartsAverageEfficiency(pawn.health.hediffSet, this.verbProps.linkedBodyPartsGroup);
				}
			}
			return 1f;
		}
		public bool IsStillUsableBy(Pawn pawn)
		{
			if (this.ownerEquipment != null && !pawn.equipment.AllEquipment.Contains(this.ownerEquipment))
			{
				return false;
			}
			if (this.ownerHediffComp != null)
			{
				bool flag = false;
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i] == this.ownerHediffComp.parent)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return this.GetDamageFactorFor(pawn) != 0f;
		}
		public virtual void ExposeData()
		{
			Scribe_Values.LookValue<int>(ref this.loadID, "loadID", 0, false);
			Scribe_Values.LookValue<VerbState>(ref this.state, "state", VerbState.Idle, false);
			Scribe_TargetInfo.LookTargetInfo(ref this.currentTarget, "currentTarget");
			Scribe_Values.LookValue<int>(ref this.burstShotsLeft, "burstShotsLeft", 0, false);
			Scribe_Values.LookValue<int>(ref this.ticksToNextBurstShot, "ticksToNextBurstShot", 0, false);
		}
		public string GetUniqueLoadID()
		{
			return "Verb_" + this.loadID;
		}
		public bool TryStartCastOn(TargetInfo castTarg, bool isSurpriseAttack = false)
		{
			if (this.caster == null)
			{
				Log.Error("Verb " + this.GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).");
				return false;
			}
			if (this.state == VerbState.Bursting || !this.CanHitTarget(castTarg))
			{
				return false;
			}
			if (this.verbProps.CausesTimeSlowdown && castTarg.HasThing && (castTarg.Thing.def.category == ThingCategory.Pawn || (castTarg.Thing.def.building != null && castTarg.Thing.def.building.IsTurret)) && castTarg.Thing.Faction == Faction.OfColony && this.caster.HostileTo(Faction.OfColony))
			{
				Find.TickManager.slower.SignalForceNormalSpeed();
			}
			this.isSurpriseAttack = isSurpriseAttack;
			this.currentTarget = castTarg;
			if (this.CasterIsPawn && this.verbProps.warmupTicks > 0)
			{
				ShootLine newShootLine;
				if (!this.TryFindShootLineFromTo(this.caster.Position, castTarg, out newShootLine))
				{
					return false;
				}
				this.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, this.caster.Position);
				float statValue = this.CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
				int ticks = Mathf.CeilToInt((float)this.verbProps.warmupTicks * statValue);
				this.CasterPawn.stances.SetStance(new Stance_Warmup(ticks, castTarg, this));
			}
			else
			{
				this.WarmupComplete();
			}
			return true;
		}
		public virtual void WarmupComplete()
		{
			this.burstShotsLeft = this.ShotsPerBurst;
			this.state = VerbState.Bursting;
			this.TryCastNextBurstShot();
		}
		public void VerbTick()
		{
			if (this.state == VerbState.Bursting)
			{
				this.ticksToNextBurstShot--;
				if (this.ticksToNextBurstShot <= 0)
				{
					this.TryCastNextBurstShot();
				}
			}
		}
		protected void TryCastNextBurstShot()
		{
			if (this.TryCastShot())
			{
				if (this.verbProps.muzzleFlashScale > 0.01f)
				{
					MoteThrower.ThrowStatic(this.caster.Position, ThingDefOf.Mote_ShotFlash, this.verbProps.muzzleFlashScale);
				}
				if (this.verbProps.soundCast != null)
				{
					this.verbProps.soundCast.PlayOneShot(this.caster.Position);
				}
				if (this.verbProps.soundCastTail != null)
				{
					this.verbProps.soundCastTail.PlayOneShotOnCamera();
				}
				if (this.CasterIsPawn)
				{
					if (this.CasterPawn.thinker == null)
					{
						return;
					}
					this.CasterPawn.mindState.Notify_EngagedTarget();
				}
				this.burstShotsLeft--;
			}
			else
			{
				this.burstShotsLeft = 0;
			}
			if (this.burstShotsLeft > 0)
			{
				this.ticksToNextBurstShot = this.verbProps.ticksBetweenBurstShots;
				if (this.CasterIsPawn)
				{
					this.CasterPawn.stances.SetStance(new Stance_Cooldown(this.verbProps.ticksBetweenBurstShots + 1, this.currentTarget));
				}
			}
			else
			{
				this.state = VerbState.Idle;
				if (this.CasterIsPawn)
				{
					this.CasterPawn.stances.SetStance(new Stance_Cooldown(this.verbProps.AdjustedCooldownTicks(this.ownerEquipment), this.currentTarget));
				}
				if (this.castCompleteCallback != null)
				{
					this.castCompleteCallback();
				}
			}
		}
		protected abstract bool TryCastShot();
		public void Notify_PickedUp()
		{
			this.state = VerbState.Idle;
			this.currentTarget = null;
			this.burstShotsLeft = 0;
			this.ticksToNextBurstShot = 0;
			this.castCompleteCallback = null;
			this.isSurpriseAttack = false;
		}
		public virtual void Notify_Dropped()
		{
		}
		public virtual float HighlightFieldRadiusAroundTarget()
		{
			return 0f;
		}
		public bool CanHitTarget(TargetInfo targ)
		{
			return this.CanHitTargetFrom(this.caster.Position, targ);
		}
		public virtual bool CanHitTargetFrom(IntVec3 root, TargetInfo targ)
		{
			if (targ.Thing != null && targ.Thing == this.caster)
			{
				return this.verbProps.targetParams.canTargetSelf;
			}
			if (this.CasterIsPawn && this.CasterPawn.apparel != null)
			{
				List<Apparel> wornApparel = this.CasterPawn.apparel.WornApparel;
				for (int i = 0; i < wornApparel.Count; i++)
				{
					if (!wornApparel[i].AllowVerbCast(root, targ))
					{
						return false;
					}
				}
			}
			ShootLine shootLine;
			return this.TryFindShootLineFromTo(root, targ, out shootLine);
		}
		public bool TryFindShootLineFromTo(IntVec3 root, TargetInfo targ, out ShootLine resultingLine)
		{
			if (this.verbProps.MeleeRange)
			{
				if (root.AdjacentTo8WayOrInside(targ))
				{
					resultingLine = new ShootLine(root, targ.Cell);
					return true;
				}
				resultingLine = new ShootLine(root, targ.Cell);
				return false;
			}
			else
			{
				float lengthHorizontalSquared = (root - targ.Cell).LengthHorizontalSquared;
				if (lengthHorizontalSquared > this.verbProps.range * this.verbProps.range || lengthHorizontalSquared < this.verbProps.minRange * this.verbProps.minRange)
				{
					resultingLine = new ShootLine(root, targ.Cell);
					return false;
				}
				if (!this.verbProps.NeedsLineOfSight)
				{
					resultingLine = new ShootLine(root, targ.Cell);
					return true;
				}
				if (this.CasterIsPawn)
				{
					IntVec3 dest;
					if (this.CanHitFromCell(root, targ, out dest))
					{
						resultingLine = new ShootLine(root, dest);
						return true;
					}
					ShootLeanUtility.LeanShootingSourcesFromTo(root, targ.Cell, Verb.tempLeanShootSources);
					for (int i = 0; i < Verb.tempLeanShootSources.Count; i++)
					{
						IntVec3 intVec = Verb.tempLeanShootSources[i];
						if (this.CanHitFromCell(intVec, targ, out dest))
						{
							resultingLine = new ShootLine(intVec, dest);
							return true;
						}
					}
				}
				else
				{
					CellRect.CellRectIterator iterator = this.caster.OccupiedRect().GetIterator();
					while (!iterator.Done())
					{
						IntVec3 current = iterator.Current;
						IntVec3 dest;
						if (this.CanHitFromCell(current, targ, out dest))
						{
							resultingLine = new ShootLine(current, dest);
							return true;
						}
						iterator.MoveNext();
					}
				}
				resultingLine = new ShootLine(root, targ.Cell);
				return false;
			}
		}
		private bool CanHitFromCell(IntVec3 sourceCell, TargetInfo targ, out IntVec3 goodDest)
		{
			if (targ.Thing != null)
			{
				ShootLeanUtility.CalcShootableCellsOf(Verb.tempDestList, targ.Thing);
				for (int i = 0; i < Verb.tempDestList.Count; i++)
				{
					if (this.CanHitCellFromCellIgnoringRange(sourceCell, Verb.tempDestList[i]))
					{
						goodDest = Verb.tempDestList[i];
						return true;
					}
				}
			}
			else
			{
				if (this.CanHitCellFromCellIgnoringRange(sourceCell, targ.Cell))
				{
					goodDest = targ.Cell;
					return true;
				}
			}
			goodDest = IntVec3.Invalid;
			return false;
		}
		private bool CanHitCellFromCellIgnoringRange(IntVec3 sourceSq, IntVec3 targetLoc)
		{
			return (!this.verbProps.mustCastOnOpenGround || (targetLoc.Standable() && !Find.ThingGrid.CellContains(targetLoc, ThingCategory.Pawn))) && (!this.verbProps.requireLineOfSight || GenSight.LineOfSight(sourceSq, targetLoc, true));
		}
		public override string ToString()
		{
			string str;
			if (!this.verbProps.label.NullOrEmpty())
			{
				str = this.verbProps.label;
			}
			else
			{
				if (this.ownerHediffComp != null)
				{
					str = this.ownerHediffComp.Def.label;
				}
				else
				{
					if (this.ownerEquipment != null)
					{
						str = this.ownerEquipment.def.label;
					}
					else
					{
						if (this.verbProps.linkedBodyPartsGroup != null)
						{
							str = this.verbProps.linkedBodyPartsGroup.defName;
						}
						else
						{
							str = "unknown";
						}
					}
				}
			}
			return base.GetType().ToString() + "(" + str + ")";
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorld
{
	public class Building_TurretGun : Building_Turret
	{
		public Thing gun;
		protected TurretTop top;
		protected CompPowerTrader powerComp;
		protected CompMannable mannableComp;
		public bool loaded;
		protected TargetInfo currentTargetInt = TargetInfo.Invalid;
		protected int burstWarmupTicksLeft;
		protected int burstCooldownTicksLeft;
		public CompEquippable GunCompEq
		{
			get
			{
				return this.gun.TryGetComp<CompEquippable>();
			}
		}
		public override TargetInfo CurrentTarget
		{
			get
			{
				return this.currentTargetInt;
			}
		}
		private bool WarmingUp
		{
			get
			{
				return this.burstWarmupTicksLeft > 0;
			}
		}
		public override Verb AttackVerb
		{
			get
			{
				if (this.gun == null)
				{
					return null;
				}
				return this.GunCompEq.verbTracker.PrimaryVerb;
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.mannableComp = base.GetComp<CompMannable>();
			this.gun = ThingMaker.MakeThing(this.def.building.turretGunDef, null);
			for (int i = 0; i < this.GunCompEq.AllVerbs.Count; i++)
			{
				Verb verb = this.GunCompEq.AllVerbs[i];
				verb.caster = this;
				verb.castCompleteCallback = new Action(this.BurstComplete);
			}
			this.top = new TurretTop(this);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.burstCooldownTicksLeft, "burstCooldownTicksLeft", 0, false);
			Scribe_Values.LookValue<bool>(ref this.loaded, "loaded", false, false);
		}
		public override void OrderAttack(TargetInfo targ)
		{
			if ((targ.Cell - base.Position).LengthHorizontal < this.GunCompEq.PrimaryVerb.verbProps.minRange)
			{
				Messages.Message("MessageTargetBelowMinimumRange".Translate(), this, MessageSound.RejectInput);
				return;
			}
			if ((targ.Cell - base.Position).LengthHorizontal > this.GunCompEq.PrimaryVerb.verbProps.range)
			{
				Messages.Message("MessageTargetBeyondMaximumRange".Translate(), this, MessageSound.RejectInput);
				return;
			}
			this.forcedTarget = targ;
		}
		public override void Tick()
		{
			base.Tick();
			if (this.powerComp != null && !this.powerComp.PowerOn)
			{
				return;
			}
			if (this.mannableComp != null && !this.mannableComp.MannedNow)
			{
				return;
			}
			this.GunCompEq.verbTracker.VerbsTick();
			if (this.stunner.Stunned)
			{
				return;
			}
			if (this.GunCompEq.PrimaryVerb.state == VerbState.Bursting)
			{
				return;
			}
			if (this.WarmingUp)
			{
				this.burstWarmupTicksLeft--;
				if (this.burstWarmupTicksLeft == 0)
				{
					this.BeginBurst();
				}
			}
			else
			{
				if (this.burstCooldownTicksLeft > 0)
				{
					this.burstCooldownTicksLeft--;
				}
				if (this.burstCooldownTicksLeft == 0)
				{
					this.TryStartShootSomething();
				}
			}
			this.top.TurretTopTick();
		}
		protected void TryStartShootSomething()
		{
			if (this.forcedTarget.ThingDestroyed)
			{
				this.forcedTarget = null;
			}
			if (this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead && Find.RoofGrid.Roofed(base.Position))
			{
				return;
			}
			bool isValid = this.currentTargetInt.IsValid;
			if (this.forcedTarget.IsValid)
			{
				this.currentTargetInt = this.forcedTarget;
			}
			else
			{
				this.currentTargetInt = this.TryFindNewTarget();
			}
			if (!isValid && this.currentTargetInt.IsValid)
			{
				SoundDefOf.TurretAcquireTarget.PlayOneShot(base.Position);
			}
			if (this.currentTargetInt.IsValid)
			{
				if (this.def.building.turretBurstWarmupTicks > 0)
				{
					this.burstWarmupTicksLeft = this.def.building.turretBurstWarmupTicks;
				}
				else
				{
					this.BeginBurst();
				}
			}
		}
		protected TargetInfo TryFindNewTarget()
		{
			Thing searcher;
			Faction faction;
			if (this.mannableComp != null && this.mannableComp.MannedNow)
			{
				searcher = this.mannableComp.ManningPawn;
				faction = this.mannableComp.ManningPawn.Faction;
			}
			else
			{
				searcher = this;
				faction = base.Faction;
			}
			if (this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead && faction.HostileTo(Faction.OfColony) && Rand.Value < 0.5f && Find.ListerBuildings.allBuildingsColonist.Count > 0)
			{
				return Find.ListerBuildings.allBuildingsColonist.RandomElement<Building>();
			}
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedThreat;
			if (!this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead)
			{
				targetScanFlags |= TargetScanFlags.NeedLOSToAll;
			}
			if (this.GunCompEq.PrimaryVerb.verbProps.ai_IsIncendiary)
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return AttackTargetFinder.BestShootTargetFromCurrentPosition(searcher, new Predicate<Thing>(this.IsValidTarget), this.GunCompEq.PrimaryVerb.verbProps.range, this.GunCompEq.PrimaryVerb.verbProps.minRange, targetScanFlags);
		}
		private bool IsValidTarget(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if (this.GunCompEq.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead)
				{
					RoofDef roofDef = Find.RoofGrid.RoofAt(t.Position);
					if (roofDef != null && roofDef.isThickRoof)
					{
						return false;
					}
				}
				if (this.mannableComp == null)
				{
					return !GenAI.MachinesLike(base.Faction, pawn);
				}
			}
			return true;
		}
		protected void BeginBurst()
		{
			this.GunCompEq.PrimaryVerb.TryStartCastOn(this.CurrentTarget, false);
		}
		protected void BurstComplete()
		{
			if (this.def.building.turretBurstCooldownTicks >= 0)
			{
				this.burstCooldownTicksLeft = this.def.building.turretBurstCooldownTicks;
			}
			else
			{
				this.burstCooldownTicksLeft = this.GunCompEq.PrimaryVerb.verbProps.defaultCooldownTicks;
			}
			this.loaded = false;
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string inspectString = base.GetInspectString();
			if (!inspectString.NullOrEmpty())
			{
				stringBuilder.AppendLine(inspectString);
			}
			stringBuilder.AppendLine("GunInstalled".Translate() + ": " + this.gun.LabelCap);
			if (this.GunCompEq.PrimaryVerb.verbProps.minRange > 0f)
			{
				stringBuilder.AppendLine("MinimumRange".Translate() + ": " + this.GunCompEq.PrimaryVerb.verbProps.minRange.ToString("F0"));
			}
			if (this.burstCooldownTicksLeft > 0)
			{
				stringBuilder.AppendLine("CanFireIn".Translate() + ": " + this.burstCooldownTicksLeft.TickstoSecondsString());
			}
			if (this.def.building.turretShellDef != null)
			{
				if (this.loaded)
				{
					stringBuilder.AppendLine("ShellLoaded".Translate());
				}
				else
				{
					stringBuilder.AppendLine("ShellNotLoaded".Translate());
				}
			}
			return stringBuilder.ToString();
		}
		public override void Draw()
		{
			this.top.DrawTurret();
			base.Draw();
		}
		public override void DrawExtraSelectionOverlays()
		{
			float range = this.GunCompEq.PrimaryVerb.verbProps.range;
			if (range < 90f)
			{
				GenDraw.DrawRadiusRing(base.Position, range);
			}
			float minRange = this.GunCompEq.PrimaryVerb.verbProps.minRange;
			if (minRange < 90f && minRange > 0.1f)
			{
				GenDraw.DrawRadiusRing(base.Position, minRange);
			}
			if (this.burstWarmupTicksLeft > 0)
			{
				int degreesWide = (int)((float)this.burstWarmupTicksLeft * 0.5f);
				GenDraw.DrawAimPie(this, this.CurrentTarget, degreesWide, (float)this.def.size.x * 0.5f);
			}
		}
		[DebuggerHidden]
		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_TurretGun.<GetGizmos>c__IteratorD1 <GetGizmos>c__IteratorD = new Building_TurretGun.<GetGizmos>c__IteratorD1();
			<GetGizmos>c__IteratorD.<>f__this = this;
			Building_TurretGun.<GetGizmos>c__IteratorD1 expr_0E = <GetGizmos>c__IteratorD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

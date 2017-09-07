using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.Sound;
namespace RimWorld
{
	public class Verb_MeleeAttack : Verb
	{
		private const int TargetCooldown = 50;
		private const float DefaultHitChance = 0.6f;
		protected override bool TryCastShot()
		{
			Pawn casterPawn = base.CasterPawn;
			if (casterPawn.stances.FullBodyBusy)
			{
				return false;
			}
			Thing thing = this.currentTarget.Thing;
			if (!base.CanHitTarget(thing))
			{
				Log.Warning(string.Concat(new object[]
				{
					casterPawn,
					" meleed ",
					thing,
					" from out of melee position."
				}));
			}
			casterPawn.Drawer.rotator.FaceCell(thing.Position);
			if (!this.IsTargetImmobile(this.currentTarget) && casterPawn.skills != null)
			{
				casterPawn.skills.Learn(SkillDefOf.Melee, 150f);
			}
			bool result;
			SoundDef soundDef;
			if (Rand.Value < this.GetHitChance(thing))
			{
				result = true;
				this.ApplyMeleeDamageToTarget(this.currentTarget);
				if (thing.def.category == ThingCategory.Building)
				{
					soundDef = this.SoundHitBuilding();
				}
				else
				{
					soundDef = this.SoundHitPawn();
				}
			}
			else
			{
				result = false;
				soundDef = this.SoundMiss();
			}
			soundDef.PlayOneShot(thing.Position);
			casterPawn.Drawer.Notify_MeleeAttackOn(thing);
			Pawn pawn = thing as Pawn;
			if (pawn != null && !pawn.Dead)
			{
				Stance_Cooldown stance_Cooldown = pawn.stances.curStance as Stance_Cooldown;
				if (stance_Cooldown == null || stance_Cooldown.ticksLeft >= 50)
				{
					pawn.stances.SetStance(new Stance_Cooldown(50, null));
				}
				if (casterPawn.MentalStateDef != MentalStateDefOf.SocialFighting || pawn.MentalStateDef != MentalStateDefOf.SocialFighting)
				{
					pawn.mindState.meleeThreat = casterPawn;
					pawn.mindState.lastMeleeThreatHarmTick = Find.TickManager.TicksGame;
				}
			}
			casterPawn.Drawer.rotator.FaceCell(thing.Position);
			if (casterPawn.caller != null)
			{
				casterPawn.caller.Notify_DidMeleeAttack();
			}
			return result;
		}
		[DebuggerHidden]
		private IEnumerable<DamageInfo> DamageInfosToApply(TargetInfo target)
		{
			Verb_MeleeAttack.<DamageInfosToApply>c__Iterator123 <DamageInfosToApply>c__Iterator = new Verb_MeleeAttack.<DamageInfosToApply>c__Iterator123();
			<DamageInfosToApply>c__Iterator.target = target;
			<DamageInfosToApply>c__Iterator.<$>target = target;
			<DamageInfosToApply>c__Iterator.<>f__this = this;
			Verb_MeleeAttack.<DamageInfosToApply>c__Iterator123 expr_1C = <DamageInfosToApply>c__Iterator;
			expr_1C.$PC = -2;
			return expr_1C;
		}
		private float GetHitChance(TargetInfo target)
		{
			if (this.isSurpriseAttack)
			{
				return 1f;
			}
			if (this.IsTargetImmobile(target))
			{
				return 1f;
			}
			if (base.CasterPawn.skills != null)
			{
				return base.CasterPawn.GetStatValue(StatDefOf.MeleeHitChance, true);
			}
			return 0.6f;
		}
		private bool IsTargetImmobile(TargetInfo target)
		{
			Thing thing = target.Thing;
			Pawn pawn = thing as Pawn;
			return thing.def.category != ThingCategory.Pawn || pawn.Downed || pawn.GetPosture() != PawnPosture.Standing;
		}
		private void ApplyMeleeDamageToTarget(TargetInfo target)
		{
			foreach (DamageInfo current in this.DamageInfosToApply(target))
			{
				if (target.ThingDestroyed)
				{
					break;
				}
				target.Thing.TakeDamage(current);
			}
		}
		private SoundDef SoundHitPawn()
		{
			if (this.ownerEquipment != null && this.ownerEquipment.Stuff != null)
			{
				if (this.verbProps.meleeDamageDef.armorCategory == DamageArmorCategory.Sharp)
				{
					if (!this.ownerEquipment.Stuff.stuffProps.soundMeleeHitSharp.NullOrUndefined())
					{
						return this.ownerEquipment.Stuff.stuffProps.soundMeleeHitSharp;
					}
				}
				else
				{
					if (!this.ownerEquipment.Stuff.stuffProps.soundMeleeHitBlunt.NullOrUndefined())
					{
						return this.ownerEquipment.Stuff.stuffProps.soundMeleeHitBlunt;
					}
				}
			}
			if (base.CasterPawn != null && !base.CasterPawn.def.race.soundMeleeHitPawn.NullOrUndefined())
			{
				return base.CasterPawn.def.race.soundMeleeHitPawn;
			}
			return SoundDefOf.Pawn_Melee_Punch_HitPawn;
		}
		private SoundDef SoundHitBuilding()
		{
			if (this.ownerEquipment != null && this.ownerEquipment.Stuff != null)
			{
				if (this.verbProps.meleeDamageDef.armorCategory == DamageArmorCategory.Sharp)
				{
					if (!this.ownerEquipment.Stuff.stuffProps.soundMeleeHitSharp.NullOrUndefined())
					{
						return this.ownerEquipment.Stuff.stuffProps.soundMeleeHitSharp;
					}
				}
				else
				{
					if (!this.ownerEquipment.Stuff.stuffProps.soundMeleeHitBlunt.NullOrUndefined())
					{
						return this.ownerEquipment.Stuff.stuffProps.soundMeleeHitBlunt;
					}
				}
			}
			if (base.CasterPawn != null && !base.CasterPawn.def.race.soundMeleeHitBuilding.NullOrUndefined())
			{
				return base.CasterPawn.def.race.soundMeleeHitBuilding;
			}
			return SoundDefOf.Pawn_Melee_Punch_HitBuilding;
		}
		private SoundDef SoundMiss()
		{
			if (base.CasterPawn != null && !base.CasterPawn.def.race.soundMeleeMiss.NullOrUndefined())
			{
				return base.CasterPawn.def.race.soundMeleeMiss;
			}
			return SoundDefOf.Pawn_Melee_Punch_Miss;
		}
	}
}

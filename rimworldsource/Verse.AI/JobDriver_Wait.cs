using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_Wait : JobDriver
	{
		private const int TargetSearchInterval = 4;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Wait.<MakeNewToils>c__Iterator127 <MakeNewToils>c__Iterator = new JobDriver_Wait.<MakeNewToils>c__Iterator127();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Wait.<MakeNewToils>c__Iterator127 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void Notify_StanceChanged()
		{
			if (this.pawn.stances.curStance is Stance_Mobile)
			{
				this.CheckForAutoAttack();
			}
		}
		private void CheckForAutoAttack()
		{
			if (this.pawn.Downed)
			{
				return;
			}
			if (this.pawn.stances.FullBodyBusy)
			{
				return;
			}
			if (this.pawn.story == null || !this.pawn.story.WorkTagIsDisabled(WorkTags.Violent))
			{
				bool flag = this.pawn.RaceProps.ToolUser && this.pawn.Faction == Faction.OfColony;
				for (int i = 0; i < 9; i++)
				{
					IntVec3 c = this.pawn.Position + GenAdj.AdjacentCellsAndInside[i];
					if (c.InBounds())
					{
						Fire fire = null;
						List<Thing> list = Find.ThingGrid.ThingsListAt(c);
						if (list != null)
						{
							for (int j = 0; j < list.Count; j++)
							{
								Pawn pawn = list[j] as Pawn;
								if (pawn != null && !pawn.Downed && this.pawn.HostileTo(pawn))
								{
									this.pawn.meleeVerbs.TryMeleeAttack(pawn, null, false);
									return;
								}
								if (flag)
								{
									Fire fire2 = list[j] as Fire;
									if (fire2 != null && fire == null && (fire2.parent == null || fire2.parent != this.pawn))
									{
										fire = fire2;
									}
								}
							}
						}
						if (flag && fire != null)
						{
							this.pawn.natives.TryBeatFire(fire);
							return;
						}
					}
				}
				if (this.pawn.Faction != null && this.pawn.jobs.curJob.def == JobDefOf.WaitCombat)
				{
					bool allowManualCastWeapons = !this.pawn.IsColonist;
					Verb verb = this.pawn.TryGetAttackVerb(allowManualCastWeapons);
					if (verb != null && !verb.verbProps.MeleeRange)
					{
						TargetScanFlags targetScanFlags = TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedThreat;
						if (verb.verbProps.ai_IsIncendiary)
						{
							targetScanFlags |= TargetScanFlags.NeedNonBurning;
						}
						Thing thing = AttackTargetFinder.BestShootTargetFromCurrentPosition(this.pawn, null, verb.verbProps.range, verb.verbProps.minRange, targetScanFlags);
						if (thing != null)
						{
							this.pawn.equipment.TryStartAttack(thing);
							return;
						}
					}
				}
			}
		}
	}
}

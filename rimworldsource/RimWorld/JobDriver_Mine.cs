using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Mine : JobDriver
	{
		public const int BaseTicksBetweenPickHits = 120;
		private const int BaseDamagePerPickHit = 80;
		private int ticksToPickHit = -1000;
		private Effecter effecter;
		private Thing Mineable
		{
			get
			{
				return base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Mine.<MakeNewToils>c__Iterator17 <MakeNewToils>c__Iterator = new JobDriver_Mine.<MakeNewToils>c__Iterator17();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Mine.<MakeNewToils>c__Iterator17 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		private void ResetTicksToPickHit()
		{
			float statValue = this.pawn.GetStatValue(StatDefOf.MiningSpeed, true);
			this.ticksToPickHit = (int)Math.Round((double)(120f / statValue));
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksToPickHit, "ticksToPickHit", 0, false);
		}
	}
}

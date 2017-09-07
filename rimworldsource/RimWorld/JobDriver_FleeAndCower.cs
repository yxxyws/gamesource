using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_FleeAndCower : JobDriver
	{
		private const TargetIndex DestInd = TargetIndex.A;
		private const int CowerTicks = 1200;
		private const int CheckFleeAgainInvervalTicks = 35;
		public override string GetReport()
		{
			if (this.pawn.Position != base.CurJob.GetTarget(TargetIndex.A).Cell)
			{
				return base.GetReport();
			}
			return "ReportCowering".Translate();
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_FleeAndCower.<MakeNewToils>c__Iterator2A <MakeNewToils>c__Iterator2A = new JobDriver_FleeAndCower.<MakeNewToils>c__Iterator2A();
			<MakeNewToils>c__Iterator2A.<>f__this = this;
			JobDriver_FleeAndCower.<MakeNewToils>c__Iterator2A expr_0E = <MakeNewToils>c__Iterator2A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

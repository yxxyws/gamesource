using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Vomit : JobDriver
	{
		private int ticksLeft;
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.ticksLeft, "ticksLeft", 0, false);
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Vomit.<MakeNewToils>c__Iterator35 <MakeNewToils>c__Iterator = new JobDriver_Vomit.<MakeNewToils>c__Iterator35();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Vomit.<MakeNewToils>c__Iterator35 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

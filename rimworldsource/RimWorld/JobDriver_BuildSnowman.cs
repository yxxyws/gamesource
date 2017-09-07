using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_BuildSnowman : JobDriver
	{
		protected const int BaseWorkAmount = 2300;
		private float workLeft = -1000f;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_BuildSnowman.<MakeNewToils>c__Iterator1D <MakeNewToils>c__Iterator1D = new JobDriver_BuildSnowman.<MakeNewToils>c__Iterator1D();
			<MakeNewToils>c__Iterator1D.<>f__this = this;
			JobDriver_BuildSnowman.<MakeNewToils>c__Iterator1D expr_0E = <MakeNewToils>c__Iterator1D;
			expr_0E.$PC = -2;
			return expr_0E;
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<float>(ref this.workLeft, "workLeft", 0f, false);
		}
	}
}

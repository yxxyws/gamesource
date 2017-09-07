using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_ClearSnow : JobDriver
	{
		private const float ClearWorkPerSnowDepth = 100f;
		private float workDone;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_ClearSnow.<MakeNewToils>c__Iterator14 <MakeNewToils>c__Iterator = new JobDriver_ClearSnow.<MakeNewToils>c__Iterator14();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_ClearSnow.<MakeNewToils>c__Iterator14 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

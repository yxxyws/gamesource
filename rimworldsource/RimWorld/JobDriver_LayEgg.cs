using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_LayEgg : JobDriver
	{
		private const int LayEgg = 500;
		private const TargetIndex LaySpotInd = TargetIndex.A;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_LayEgg.<MakeNewToils>c__Iterator0 <MakeNewToils>c__Iterator = new JobDriver_LayEgg.<MakeNewToils>c__Iterator0();
			JobDriver_LayEgg.<MakeNewToils>c__Iterator0 expr_07 = <MakeNewToils>c__Iterator;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}

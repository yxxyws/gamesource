using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_Kill : JobDriver
	{
		private const TargetIndex VictimInd = TargetIndex.A;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Kill.<MakeNewToils>c__Iterator12A <MakeNewToils>c__Iterator12A = new JobDriver_Kill.<MakeNewToils>c__Iterator12A();
			<MakeNewToils>c__Iterator12A.<>f__this = this;
			JobDriver_Kill.<MakeNewToils>c__Iterator12A expr_0E = <MakeNewToils>c__Iterator12A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

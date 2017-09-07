using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_Goto : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Goto.<MakeNewToils>c__Iterator125 <MakeNewToils>c__Iterator = new JobDriver_Goto.<MakeNewToils>c__Iterator125();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Goto.<MakeNewToils>c__Iterator125 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

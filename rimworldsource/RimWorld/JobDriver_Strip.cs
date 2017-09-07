using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Strip : JobDriver
	{
		private const int StripTicks = 60;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Strip.<MakeNewToils>c__Iterator32 <MakeNewToils>c__Iterator = new JobDriver_Strip.<MakeNewToils>c__Iterator32();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_Strip.<MakeNewToils>c__Iterator32 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

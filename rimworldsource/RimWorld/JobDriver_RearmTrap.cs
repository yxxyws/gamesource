using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_RearmTrap : JobDriver
	{
		private const int RearmTicks = 750;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_RearmTrap.<MakeNewToils>c__Iterator18 <MakeNewToils>c__Iterator = new JobDriver_RearmTrap.<MakeNewToils>c__Iterator18();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_RearmTrap.<MakeNewToils>c__Iterator18 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

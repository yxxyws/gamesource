using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Repair : JobDriver
	{
		private const float TicksBetweenRepairs = 12f;
		protected float ticksToNextRepair;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Repair.<MakeNewToils>c__IteratorC <MakeNewToils>c__IteratorC = new JobDriver_Repair.<MakeNewToils>c__IteratorC();
			<MakeNewToils>c__IteratorC.<>f__this = this;
			JobDriver_Repair.<MakeNewToils>c__IteratorC expr_0E = <MakeNewToils>c__IteratorC;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

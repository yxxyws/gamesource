using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Verse.AI
{
	public class JobDriver_TakeInventory : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_TakeInventory.<MakeNewToils>c__Iterator126 <MakeNewToils>c__Iterator = new JobDriver_TakeInventory.<MakeNewToils>c__Iterator126();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_TakeInventory.<MakeNewToils>c__Iterator126 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

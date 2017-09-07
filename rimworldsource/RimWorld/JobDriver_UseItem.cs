using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_UseItem : JobDriver
	{
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_UseItem.<MakeNewToils>c__Iterator1C <MakeNewToils>c__Iterator1C = new JobDriver_UseItem.<MakeNewToils>c__Iterator1C();
			JobDriver_UseItem.<MakeNewToils>c__Iterator1C expr_07 = <MakeNewToils>c__Iterator1C;
			expr_07.$PC = -2;
			return expr_07;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Execute : JobDriver
	{
		protected Pawn Victim
		{
			get
			{
				return (Pawn)base.CurJob.targetA.Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Execute.<MakeNewToils>c__Iterator3C <MakeNewToils>c__Iterator3C = new JobDriver_Execute.<MakeNewToils>c__Iterator3C();
			<MakeNewToils>c__Iterator3C.<>f__this = this;
			JobDriver_Execute.<MakeNewToils>c__Iterator3C expr_0E = <MakeNewToils>c__Iterator3C;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_ChatWithPrisoner : JobDriver
	{
		protected Pawn Talkee
		{
			get
			{
				return (Pawn)base.CurJob.targetA.Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_ChatWithPrisoner.<MakeNewToils>c__Iterator3B <MakeNewToils>c__Iterator3B = new JobDriver_ChatWithPrisoner.<MakeNewToils>c__Iterator3B();
			<MakeNewToils>c__Iterator3B.<>f__this = this;
			JobDriver_ChatWithPrisoner.<MakeNewToils>c__Iterator3B expr_0E = <MakeNewToils>c__Iterator3B;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

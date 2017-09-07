using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_BeatFire : JobDriver
	{
		protected Fire TargetFire
		{
			get
			{
				return (Fire)base.CurJob.targetA.Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_BeatFire.<MakeNewToils>c__IteratorD <MakeNewToils>c__IteratorD = new JobDriver_BeatFire.<MakeNewToils>c__IteratorD();
			<MakeNewToils>c__IteratorD.<>f__this = this;
			JobDriver_BeatFire.<MakeNewToils>c__IteratorD expr_0E = <MakeNewToils>c__IteratorD;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_PlayBilliards : JobDriver
	{
		private const int ShotDuration = 600;
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_PlayBilliards.<MakeNewToils>c__Iterator1F <MakeNewToils>c__Iterator1F = new JobDriver_PlayBilliards.<MakeNewToils>c__Iterator1F();
			<MakeNewToils>c__Iterator1F.<>f__this = this;
			JobDriver_PlayBilliards.<MakeNewToils>c__Iterator1F expr_0E = <MakeNewToils>c__Iterator1F;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

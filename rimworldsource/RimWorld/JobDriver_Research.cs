using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_Research : JobDriver
	{
		private const int JobEndInterval = 4000;
		private ResearchProjectDef Project
		{
			get
			{
				return Find.ResearchManager.currentProj;
			}
		}
		private Building_ResearchBench ResearchBench
		{
			get
			{
				return (Building_ResearchBench)base.TargetThingA;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_Research.<MakeNewToils>c__Iterator1A <MakeNewToils>c__Iterator1A = new JobDriver_Research.<MakeNewToils>c__Iterator1A();
			<MakeNewToils>c__Iterator1A.<>f__this = this;
			JobDriver_Research.<MakeNewToils>c__Iterator1A expr_0E = <MakeNewToils>c__Iterator1A;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
namespace RimWorld
{
	public abstract class JobDriver_GatherAnimalBodyResources : JobDriver
	{
		protected const TargetIndex AnimalInd = TargetIndex.A;
		protected abstract int Duration
		{
			get;
		}
		protected abstract CompHasGatherableBodyResource GetComp(Pawn animal);
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_GatherAnimalBodyResources.<MakeNewToils>c__Iterator4 <MakeNewToils>c__Iterator = new JobDriver_GatherAnimalBodyResources.<MakeNewToils>c__Iterator4();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_GatherAnimalBodyResources.<MakeNewToils>c__Iterator4 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
namespace RimWorld
{
	public class JobDriver_VisitGrave : JobDriver
	{
		private const TargetIndex GraveIndex = TargetIndex.A;
		private Building_Grave Grave
		{
			get
			{
				return (Building_Grave)base.CurJob.GetTarget(TargetIndex.A).Thing;
			}
		}
		[DebuggerHidden]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_VisitGrave.<MakeNewToils>c__Iterator25 <MakeNewToils>c__Iterator = new JobDriver_VisitGrave.<MakeNewToils>c__Iterator25();
			<MakeNewToils>c__Iterator.<>f__this = this;
			JobDriver_VisitGrave.<MakeNewToils>c__Iterator25 expr_0E = <MakeNewToils>c__Iterator;
			expr_0E.$PC = -2;
			return expr_0E;
		}
	}
}
